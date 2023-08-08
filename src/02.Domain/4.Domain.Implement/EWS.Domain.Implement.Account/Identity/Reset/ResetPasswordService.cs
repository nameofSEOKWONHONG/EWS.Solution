using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Domain.Implement.Account.Identity.Reset;

public class ResetPasswordService : ScopeServiceImpl<ResetPasswordService, ResetPasswordRequest, IResultBase>, IResetPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    public ResetPasswordService(IHttpContextAccessor accessor) : base(accessor)
    {
        _passwordHasher = this.Accessor.HttpContext.RequestServices.GetRequiredService<IPasswordHasher<User>>();
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var userSet = this.DbContext.Set<User>();
        var user = await userSet.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && 
                                                           m.Email == this.Request.Email);
        if (user.xIsEmpty())
        {
            this.Result = await JResult.FailAsync("An Error has occured!");
            return;
        }
        
        if (user.ConcurrencyStamp != string.Empty)
        {
            var verificationResult =
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, this.Request.Password);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                var hashPassword = _passwordHasher.HashPassword(user, this.Request.ConfirmPassword);
                user.PasswordHash = hashPassword;
                userSet.Update(user);
                await this.DbContext.SaveChangesAsync();
                
                this.Result = await JResult.SuccessAsync("Password Reset Successful!");
                return;
            }
        }
        this.Result = await JResult.FailAsync("An Error has occured!");
    }
}