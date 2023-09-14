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

public class RemoveUserService : ServiceImplBase<RemoveUserService, string, IResultBase>, IRemoveUserService
{
    public RemoveUserService(DbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var userSet = Db.Set<User>();
        var user = await userSet.FirstOrDefaultAsync(m => m.TenantId == Context.TenantId && m.Id == this.Request);
        if (user.IsActive.xIsTrue())
        {
            user.IsActive = !user.IsActive;
            userSet.Update(user);
            await Db.SaveChangesAsync();
        }
        else
        {
            userSet.Remove(user);
            await Db.SaveChangesAsync();
        }
        
        this.Result = await JResult.SuccessAsync();
    }
}