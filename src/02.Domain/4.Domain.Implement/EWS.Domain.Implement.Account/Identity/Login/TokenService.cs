using System.Transactions;
using EWS.Application.Const;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Domain.Implement.Account.Identity;

public class TokenService : ScopeServiceImpl<TokenService, TokenRequest, IResultBase<TokenResponse>>, ITokenService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    public TokenService(IHttpContextAccessor accessor) : base(accessor)
    {
        _passwordHasher = this.Accessor.HttpContext!.RequestServices.GetRequiredService<IPasswordHasher<User>>();
    }

    public override async Task<bool> OnExecutingAsync(ISessionContext context)
    {
        var users = this.DbContext.Set<User>();
        var user = await users.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && m.Email == this.Request.Email);
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
            await this.DbContext.SaveChangesAsync();
            this.Result = await JResult<TokenResponse>.FailAsync( "Access failed.");
            return false;
        }

        return true;
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var users = this.DbContext.Set<User>();
        var user = await users.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && m.Email == this.Request.Email);

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
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor))
        {
            sr.Register<IGetRefreshTokenService, User, string>()
                .AddFilter(() => user.xIsNotEmpty())
                .SetParameter(() => user)
                .Executed(res =>
                {
                    user.RefreshToken = res;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
                });
        
            sr.Register<IGenerateJwtTokenService, User, string>()
                .AddFilter(() => user.xIsNotEmpty())
                .SetParameter(() => user)
                .Executed(res =>
                {
                    token = res;
                });
        
            await sr.ExecuteAsync();            
        }

        var response = new TokenResponse()
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            UserImageURL = string.Empty
        };
        this.Result = await JResult<TokenResponse>.SuccessAsync(response);

        user.AccessFailedCount = 0;
        users.Update(user);
        await this.DbContext.SaveChangesAsync();
    }
}