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

public class GetUserService : ScopeServiceImpl<GetUserService, string, IResultBase<User>>, IGetUserService
{
    public GetUserService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        if (this.Request.xIsEmpty())
        {
            this.Result = JResult<User>.Fail("id is empty.");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var userSet = this.DbContext.Set<User>();
        var user = await userSet.FirstOrDefaultAsync(m => m.TenantId == context.TenantId && m.Id == this.Request);
        this.Result = await JResult<User>.SuccessAsync(user);
    }
}