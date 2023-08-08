using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Users;
using EWS.Domain.Base;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Account.Users;

public class UpdateUserService : ScopeServiceImpl<UpdateUserService, User, IResultBase>, IUpdateUserService 
{
    public UpdateUserService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var userSet = this.DbContext.Set<User>();
        userSet.Update(this.Request);
        await this.DbContext.SaveChangesAsync();
        this.Result = await JResult.SuccessAsync();
    }
}