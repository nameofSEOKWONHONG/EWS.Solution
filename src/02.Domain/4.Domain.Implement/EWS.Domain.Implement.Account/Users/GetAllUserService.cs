using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Users;
using EWS.Domain.Account;
using EWS.Domain.Account.Users;
using EWS.Domain.Base;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Users;

public class GetAllUsersService : ServiceImplBase<GetAllUsersService, GetAllUsersRequest, JPaginatedResult<UserResult>>, IGetAllUsersService
{
    public GetAllUsersService(DbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var userSet = Db.Set<User>();
        var query = userSet.AsNoTracking().Where(m => m.TenantId == Context.TenantId &&
                            m.UserName.Contains(this.Request.UserName) &&
                            m.Email.Contains(this.Request.Email));
        var total = await query.CountAsync();
        var users = await query.Skip(this.Request.PageNumber * this.Request.PageSize).Take(this.Request.PageSize).ToListAsync();
        var userResult = users.Adapt<List<UserResult>>();
        this.Result =
            await JPaginatedResult<UserResult>.SuccessAsync(userResult, total, Request.PageNumber, Request.PageSize);
    }
}