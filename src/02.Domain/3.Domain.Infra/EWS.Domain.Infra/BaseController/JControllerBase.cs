using System.Transactions;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra;

/// <summary>
/// 공통
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class JControllerBase : ControllerBase
{
    protected readonly IHttpContextAccessor Accessor;

    protected JControllerBase(IHttpContextAccessor accessor)
    {
        this.Accessor = accessor;
    }

    protected async Task<TResult> CreateServiceRouter<TDbContext, TService, TRequest, TResult>(TRequest request)
        where TDbContext : DbContext
        where TService : IServiceImplBase<TRequest, TResult>
    {
        TResult results = default;
        using var sr = ServiceRouter.Create<TDbContext>(this.Accessor, TransactionScopeOption.Suppress);
        var now = DateTime.Now;
        sr.Register<TService, TRequest, TResult>()
            .AddFilter(() => request.xIsNotEmpty())
            .SetParameter(() => request)
            .Executed(res =>
            {
                results = res;
            });

        await sr.ExecuteAsync();

        return results;
    }
}