using System.Security.Claims;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Domain.Infrastructure;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Implement.Account.Identity;

public class TokenRefreshService : ServiceImplBase<TokenRefreshService, RefreshTokenRequest, IResultBase<TokenResponse>>, ITokenRefreshService
{
    private readonly IGetPrincipalFromExpiredTokenService _getPrincipalFromExpiredTokenService;
    private readonly IGetSigningCredentialsService _getSigningCredentialsService;
    private readonly IGetClaimsService _getClaimsService;
    private readonly IGenerateEncryptedTokenService _generateEncryptedTokenService;
    private readonly IGetRefreshTokenService _getRefreshTokenService;
    public TokenRefreshService(EWSMsDbContext dbContext, 
        IGetPrincipalFromExpiredTokenService getPrincipalFromExpiredTokenService,
        IGetSigningCredentialsService getSigningCredentialsService,
        IGetClaimsService getClaimsService,
        IGenerateEncryptedTokenService generateEncryptedTokenService,
        IGetRefreshTokenService getRefreshTokenService) : base(dbContext, null)
    {
        _getPrincipalFromExpiredTokenService = getPrincipalFromExpiredTokenService;
        _getSigningCredentialsService = getSigningCredentialsService;
        _getClaimsService = getClaimsService;
        _generateEncryptedTokenService = generateEncryptedTokenService;
        _getRefreshTokenService = getRefreshTokenService;
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var users = Db.Set<User>();
        if (this.Request.xIsEmpty())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Invalid Client Token.");
            return;
        }

        ClaimsPrincipal userPrincipal = null;
        await _getPrincipalFromExpiredTokenService.Create<IGetPrincipalFromExpiredTokenService, string, ClaimsPrincipal>()
            .AddFilter(() => this.Request.xIsNotEmpty())
            .AddFilter(() => this.Request.Token.xIsNotEmpty())
            .SetParameter(() => this.Request.Token)
            .OnExecuted((res) => userPrincipal = res);
        
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await users.AsNoTracking().FirstOrDefaultAsync(m => m.TenantId == this.Request.TenantId && m.Email == userEmail);

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
        SigningCredentials signingCredentials = null;
        IEnumerable<Claim> claims = null;        
        await _getSigningCredentialsService.Create<IGetSigningCredentialsService, bool, SigningCredentials>()
            .AddFilter(() => true)
            .SetParameter(() => true)
            .OnExecuted((res) => signingCredentials = res);
        
        await _getClaimsService.Create<IGetClaimsService, User, IEnumerable<Claim>>()
            .AddFilter(() => user.xIsNotEmpty())
            .SetParameter(() => user)
            .OnExecuted((res) => claims = res);
        
        await _generateEncryptedTokenService.Create<IGenerateEncryptedTokenService, IdentityGenerateEncryptedTokenRequest, string>()
            .AddFilter(() => claims.xIsNotEmpty())
            .SetParameter(() => new IdentityGenerateEncryptedTokenRequest()
            {
                SigningCredentials = signingCredentials,
                Claims = claims
            })
            .OnExecuted((res) => token = res);
        
        await _getRefreshTokenService.Create<IGetRefreshTokenService, User, string>()
            .AddFilter(() => user.xIsNotEmpty())
            .SetParameter(() => user)
            .OnExecuted((res) => user.RefreshToken = res);

        if (token.xIsNotEmpty())
        {
            users.Update(user);
            await Db.SaveChangesAsync();
                    
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            this.Result = await JResult<TokenResponse>.SuccessAsync(response);    
        }
    }
}