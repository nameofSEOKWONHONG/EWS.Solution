using EWS.Application.Const;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Domain.Infra.Extension;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.Extentions;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Domain.Implement.Account.Identity;

public class RegisterService : ScopeServiceImpl<RegisterService, RegisterRequest, IResultBase>, IRegisterService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    public RegisterService(IHttpContextAccessor accessor) : base(accessor)
    {
        _passwordHasher = this.Accessor.HttpContext.RequestServices.GetRequiredService<IPasswordHasher<User>>();
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        var userSet = dbContext.Set<User>();
        var userWithSameUserName = await userSet.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && m.UserName == this.Request.UserName);
        if (userWithSameUserName.xIsNotEmpty())
        {
            this.Result = await JResult.FailAsync($"User name {userWithSameUserName} is already taken.");
            return;
        }
        
        var user = new User()
        {
            TenantId       = context.TenantId,
            Email          = this.Request.Email,
            FirstName      = this.Request.FirstName,
            LastName       = this.Request.LastName,
            UserName       = this.Request.UserName,
            PhoneNumber    = this.Request.PhoneNumber.vToAESEncrypt(),
            // DeptName    = request.DeptName,
            // LvlName     = request.LvlName,
            IsActive       = true,
            EmailConfirmed = true,
            // Memo           = this.Request.Memo
        };

        if (this.Request.PhoneNumber.xIsNotEmpty())
        {
            var userWithSamePhoneNumber = await userSet.FirstOrDefaultAsync(x => x.TenantId == context.TenantId && 
                x.PhoneNumber == this.Request.PhoneNumber.vToAESEncrypt());
            if (userWithSamePhoneNumber.xIsNotEmpty())
            {
                this.Result =
                    await JResult.FailAsync($"Phone number {userWithSamePhoneNumber} is already registered.");
                return;
            }
        }
        
        var userWithSameEmail = await userSet.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && m.Email == this.Request.Email);
        if (userWithSameEmail.xIsNotEmpty())
        {
            this.Result = await JResult.FailAsync($"User email {userWithSameEmail} is already registered.");
        }

        var hashedPassword = _passwordHasher.HashPassword(user, Request.Password);
        user.PasswordHash = hashedPassword;
        user.SecurityStamp = $"{user.Id}{user.FirstName}{user.LastName}{user.Email}{context.CurrentTimeAccessor.Now}".xGetHashCode();
        await userSet.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var roleDb = dbContext.Set<Role>(); 
        var basicRole = await roleDb.FirstAsync(m => m.TenantId == context.TenantId &&
                                                       m.Name == RoleConstants.BasicRole);

        var userRoleSet = dbContext.Set<UserRole>();
        await userRoleSet.AddAsync(new UserRole()
        {
            TenantId = context.TenantId,
            UserId = user.Id,
            RoleId = basicRole.Id
        });
        await dbContext.SaveChangesAsync();

        if (this.Request.AutoConfirmEmail.xIsFalse())
        {
            //todo : 메일 발송 구현 되어야 함.

            this.Result =
                await JResult<string>.SuccessAsync($"User {user.UserName} registered. Please check your mailbox to verify.");
            return;
        }

        this.Result = await JResult<string>.SuccessAsync($"User {user.UserName} registered.");
    }
}