using System.Security.Claims;
using System.Transactions;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Domain.Infra.Extension;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Implement.Account.Identity;

public class TokenReflashService : ScopeServiceImpl<TokenReflashService, RefreshTokenRequest, IResultBase<TokenResponse>>, ITokenReflashService
{
    public TokenReflashService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var users = this.DbContext.Set<User>();
        if (this.Request.xIsEmpty())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Invalid Client Token.");
            return;
        }

        ClaimsPrincipal userPrincipal = null;
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor, TransactionScopeOption.Suppress))
        {
            sr.Register<IGetPrincipalFromExpiredTokenService, string, ClaimsPrincipal>()
                .AddFilter(() => this.Request.xIsNotEmpty())
                .AddFilter(() => this.Request.Token.xIsNotEmpty())
                .SetParameter(() => this.Request.Token)
                .Executed(res => userPrincipal = res);

            await sr.ExecuteAsync();
        }
        
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await users.FirstOrDefaultAsync(m => m.TenantId == this.Request.TenantId && m.Email == userEmail);

        if (user.xIsEmpty())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("User Not Found.");
            return;
        }

        if (user.RefreshToken != this.Request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Invalid Client Token.");
            return;
        }
        
        string token = string.Empty;
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor, TransactionScopeOption.Suppress))
        {
            SigningCredentials signingCredentials = null;
            IEnumerable<Claim> claims = null;
            sr.Register<IGetSigningCredentialsService, bool, SigningCredentials>()
                .AddFilter(() => true)
                .SetParameter(() => true)
                .Executed(res => signingCredentials = res);

            sr.Register<IGetClaimsService, User, IEnumerable<Claim>>()
                .AddFilter(() => user.xIsNotEmpty())
                .SetParameter(() => user)
                .Executed(res => claims = res);
            
            sr.Register<IGenerateEncryptedToken, IdentityGenerateEncryptedTokenRequest, string>()
                .AddFilter(() => claims.xIsNotEmpty())
                .SetParameter(() => new IdentityGenerateEncryptedTokenRequest()
                {
                    SigningCredentials = signingCredentials,
                    Claims = claims
                })
                .Executed(res => token = res);

            sr.Register<IGetRefreshTokenService, User, string>()
                .AddFilter(() => user.xIsNotEmpty())
                .SetParameter(() => user)
                .Executed(res => user.RefreshToken = res);

            sr.Register()
                .AddFilter(() => token.xIsNotEmpty())
                .Executed(async () =>
                {
                    users.Update(user);
                    await this.DbContext.SaveChangesAsync();
                    
                    var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
                    this.Result = await JResult<TokenResponse>.SuccessAsync(response);                    
                });
            
            await sr.ExecuteAsync();
        }
    }
}