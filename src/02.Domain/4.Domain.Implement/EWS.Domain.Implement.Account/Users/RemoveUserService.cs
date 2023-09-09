using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Users;
using EWS.Domain.Base;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Users;

public class RemoveUserService : ScopeServiceImpl<RemoveUserService, string, IResultBase>, IRemoveUserService
{
    public RemoveUserService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        var userSet = dbContext.Set<User>();
        var user = await userSet.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && m.Id == this.Request);
        if (user.IsActive.xIsTrue())
        {
            user.IsActive = !user.IsActive;
            userSet.Update(user);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            userSet.Remove(user);
            await dbContext.SaveChangesAsync();
        }
        
        this.Result = await JResult.SuccessAsync();
    }
}