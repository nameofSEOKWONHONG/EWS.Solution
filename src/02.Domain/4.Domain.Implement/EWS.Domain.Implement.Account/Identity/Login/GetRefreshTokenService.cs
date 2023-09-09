using System.Security.Cryptography;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Identity;

public class GetRefreshTokenService : ScopeServiceImpl<GetRefreshTokenService, User, string>, IGetRefreshTokenService
{
    public GetRefreshTokenService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        this.Result = refreshToken;
        return Task.CompletedTask;
    }
}