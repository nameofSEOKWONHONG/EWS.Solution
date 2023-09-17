using EWS.Application.Const;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Domain.Infrastructure;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Identity;

public class TokenService : ServiceImplBase<TokenService, TokenRequest, IResultBase<TokenResponse>>, ITokenService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IGetRefreshTokenService _getRefreshTokenService;
    private readonly IGenerateJwtTokenService _generateJwtTokenService;
    public TokenService(EWSMsDbContext dbContext, 
        ISessionContext context, 
        IPasswordHasher<User> passwordHasher,
        IGetRefreshTokenService getRefreshTokenService,
        IGenerateJwtTokenService generateJwtTokenService) : base(dbContext, context)
    {
        _passwordHasher = passwordHasher;
        _getRefreshTokenService = getRefreshTokenService;
        _generateJwtTokenService = generateJwtTokenService;
    }

    public override async Task<bool> OnExecutingAsync()
    {
        var users = Db.Set<User>();
        var user = await users.AsNoTracking().FirstOrDefaultAsync(m => m.TenantId == Context.TenantId && m.Email == this.Request.Email);
        if (user.xIsEmpty())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Access failed.");
            return false;
        }

        if (user.AccessFailedCount > ApplicationConstants.Limit.ACCESS_LIMIT_COUNT)
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Access failed 5times. Contact administrator.");
            return false;
        }
        
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, this.Request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            user.AccessFailedCount += 1;
            users.Update(user);
            await Db.SaveChangesAsync();
            this.Result = await JResult<TokenResponse>.FailAsync( "Access failed.");
            return false;
        }

        return true;
    }

    public override async Task OnExecuteAsync()
    {
        var users = Db.Set<User>();
        var user = await users.AsNoTracking().FirstOrDefaultAsync(m => m.TenantId == Context.TenantId && m.Email == this.Request.Email);

        if (user.xIsEmpty())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("User not found.");
            return;
        }

        if (user.AccessFailedCount > ApplicationConstants.Limit.ACCESS_LIMIT_COUNT)
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Access failed 5times. Contact administrator.");
            return;
        }

        if (user.IsActive.xIsFalse())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("User not active. Please contact the administrator.");
            return;
        }

        if (user.EmailConfirmed.xIsFalse())
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Email not confirmed.");
            return;
        }

        if (user.TenantId != this.Request.TenantId)
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Tenant id not matched.");
            return;
        }

        var pwdValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, this.Request.Password);
        if (pwdValid != PasswordVerificationResult.Success)
        {
            this.Result = await JResult<TokenResponse>.FailAsync("Invalid credentials.");
            return;
        }

        string token = string.Empty;
        await _getRefreshTokenService.Create<IGetRefreshTokenService, User, string>()
            .AddFilter(() => user.xIsNotEmpty())
            .SetParameter(() => user)
            .OnExecuted((res) =>
            {
                user.RefreshToken = res;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            });

        await _generateJwtTokenService.Create<IGenerateJwtTokenService, User, string>()
            .AddFilter(() => user.xIsNotEmpty())
            .SetParameter(() => user)
            .OnExecuted((res) => token = res);

        var response = new TokenResponse()
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            UserImageURL = string.Empty
        };
        
        this.Result = await JResult<TokenResponse>.SuccessAsync(response);

        user.AccessFailedCount = 0;
        users.Update(user);
        await Db.SaveChangesAsync();
    }
}