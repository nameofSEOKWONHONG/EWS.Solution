using System.Security.Claims;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Domain.Infrastructure;
using EWS.Entity;
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
    public TokenRefreshService(DbContext dbContext, 
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
        await ServiceLoader<IGetPrincipalFromExpiredTokenService, string, ClaimsPrincipal>.Create(_getPrincipalFromExpiredTokenService)
            .AddFilter(() => this.Request.xIsNotEmpty())
            .AddFilter(() => this.Request.Token.xIsNotEmpty())
            .SetParameter(() => this.Request.Token)
            .OnExecuted((res, v) => userPrincipal = res);
        
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
        SigningCredentials signingCredentials = null;
        IEnumerable<Claim> claims = null;        
        await ServiceLoader<IGetSigningCredentialsService, bool, SigningCredentials>.Create(_getSigningCredentialsService)
            .AddFilter(() => true)
            .SetParameter(() => true)
            .OnExecuted((res, v) => signingCredentials = res);
        
        await ServiceLoader<IGetClaimsService, User, IEnumerable<Claim>>.Create(_getClaimsService)
            .AddFilter(() => user.xIsNotEmpty())
            .SetParameter(() => user)
            .OnExecuted((res, v) => claims = res);
        
        await ServiceLoader<IGenerateEncryptedTokenService, IdentityGenerateEncryptedTokenRequest, string>.Create(_generateEncryptedTokenService)
            .AddFilter(() => claims.xIsNotEmpty())
            .SetParameter(() => new IdentityGenerateEncryptedTokenRequest()
            {
                SigningCredentials = signingCredentials,
                Claims = claims
            })
            .OnExecuted((res, v) => token = res);
        
        await ServiceLoader<IGetRefreshTokenService, User, string>.Create(_getRefreshTokenService)
            .AddFilter(() => user.xIsNotEmpty())
            .SetParameter(() => user)
            .OnExecuted((res, v) => user.RefreshToken = res);

        if (token.xIsNotEmpty())
        {
            users.Update(user);
            await Db.SaveChangesAsync();
                    
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            this.Result = await JResult<TokenResponse>.SuccessAsync(response);    
        }
    }
}