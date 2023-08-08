using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Implement;

public sealed class SessionServiceExecutor<TContext, TService, TRequest, TResult>
    where TService : IServiceImplBase<TRequest, TResult>
    where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly ISessionContext _currentSessionContext;
    private readonly TService _service;
    private readonly List<Func<bool>> _filters = new();
    private Func<TRequest> _setParameter;
    private Action<TResult> _executed;

    public SessionServiceExecutor(TContext dbContext, ISessionContext currentSessionContext, TService service)
    {
        _dbContext = dbContext;
        _currentSessionContext = currentSessionContext;
        _service = service;
    }

    public SessionServiceExecutor<TContext, TService, TRequest, TResult> AddFilter(Func<bool> filter)
    {
        // Logic for adding filter
        _filters.Add(filter);
        return this;
    }

    public SessionServiceExecutor<TContext, TService, TRequest, TResult> SetParameter(Func<TRequest> parameter)
    {
        // Logic for setting parameter
        _setParameter = parameter;
        return this;
    }
    
    public SessionServiceExecutor<TContext, TService, TRequest, TResult> Executed(Action<TResult> executed)
    {
        // Logic for setting executed action
        _executed = executed;
        return this;
    }
    
    public async Task ExecuteAsync()
    {
        foreach (var filter in _filters)
        {
            var filtered = filter.Invoke();
            if(filtered.xIsFalse()) return;
        }

        var parameter = _setParameter.Invoke();
        var db = _dbContext;
        _service.DbContext = db;
        _service.Request = parameter;
        var executing = await _service.OnExecutingAsync(_currentSessionContext);
        if (executing.xIsFalse()) return;
        await _service.OnExecuteAsync(_currentSessionContext);
        _executed.Invoke(_service.Result);
    }
}