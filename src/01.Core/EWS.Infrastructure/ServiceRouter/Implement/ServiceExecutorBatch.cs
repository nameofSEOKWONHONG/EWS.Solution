using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Infrastructure.ServiceRouter.Implement;

public sealed class ServiceExecutorBatch<TContext, TService, TRequest, TResult>
    where TService : IServiceImplBase<TRequest, TResult>
    where TContext : DbContext
{
    private readonly IHttpContextAccessor _accessor;
    private readonly List<Func<TRequest, bool>> _filters = new();
    private Func<TRequest, TRequest> _setParameter;
    private Action<TResult> _executed;

    public ServiceExecutorBatch(IHttpContextAccessor accessor)
    {
        this._accessor = accessor;
    }
    
    public ServiceExecutorBatch<TContext, TService, TRequest, TResult> AddFilter(Func<TRequest, bool> filter)
    {
        // Logic for adding filter
        _filters.Add(filter);
        return this;
    }

    public ServiceExecutorBatch<TContext, TService, TRequest, TResult> SetParameter(Func<TRequest, TRequest> parameter)
    {
        // Logic for setting parameter
        _setParameter = parameter;
        return this;
    }
    
    public ServiceExecutorBatch<TContext, TService, TRequest, TResult> Executed(Action<TResult> executed)
    {
        // Logic for setting executed action
        _executed = executed;
        return this;
    }

    public async Task ExecuteAsync(TRequest request)
    {
        foreach (var filter in _filters)
        {
            var filtered = filter.Invoke(request);
            if(filtered.xIsFalse()) return;
        }

        var service = _accessor.HttpContext.RequestServices.GetRequiredService<TService>();
        service.DbContext = _accessor.HttpContext.RequestServices.GetRequiredService<TContext>();
        var session = _accessor.HttpContext.RequestServices.GetRequiredService<ISessionContext>();
        service.Request = _setParameter.Invoke(request);
        var executing = await service.OnExecutingAsync(session);
        if (executing.xIsFalse()) return;
        await service.OnExecuteAsync(session);
        _executed.Invoke(service.Result);
        service.DbContext.ChangeTracker.Clear();
    }
}