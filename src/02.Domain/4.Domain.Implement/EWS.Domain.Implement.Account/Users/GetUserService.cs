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

public class GetUserService : ServiceImplBase<GetUserService, string, IResultBase<User>>, IGetUserService
{
    public GetUserService(DbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        if (this.Request.xIsEmpty())
        {
            this.Result = JResult<User>.Fail("id is empty.");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var userSet = Db.Set<User>();
        var user = await userSet.FirstOrDefaultAsync(m => m.TenantId == Context.TenantId && m.Id == this.Request);
        this.Result = await JResult<User>.SuccessAsync(user);
    }
}