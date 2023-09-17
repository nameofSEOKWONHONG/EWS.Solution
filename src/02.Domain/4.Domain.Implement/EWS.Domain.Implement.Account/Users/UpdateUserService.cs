using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Users;
using EWS.Domain.Base;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Users;

public class UpdateUserService : ServiceImplBase<UpdateUserService, User, IResultBase>, IUpdateUserService 
{
    public UpdateUserService(EWSMsDbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var userSet = Db.Set<User>();
        userSet.Update(this.Request);
        await Db.SaveChangesAsync();
        this.Result = await JResult.SuccessAsync();
    }
}