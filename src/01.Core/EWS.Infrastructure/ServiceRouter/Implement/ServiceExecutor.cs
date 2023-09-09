using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Infrastructure.ServiceRouter.Implement;

public sealed class SimpleServiceExecutor
{
    private readonly IHttpContextAccessor _accessor;
    private readonly List<Func<bool>> _filters = new();
    private Func<Task> _executed;
    
    public SimpleServiceExecutor(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    
    public SimpleServiceExecutor AddFilter(Func<bool> filter)
    {
        // Logic for adding filter
        _filters.Add(filter);
        return this;
    }
    
    public SimpleServiceExecutor Executed(Func<Task> executed)
    {
        // Logic for setting executed action
        _executed = executed;
        return this;
    }

    public async Task ExecuteAsync()
    {
        foreach (var filter in _filters.xToList())
        {
            var filtered = filter.Invoke();
            if(filtered.xIsFalse()) return;
        }
        await _executed.Invoke();
    }
}

public sealed class ServiceExecutor<TDbContext, TService, TRequest, TResult>
    where TService : IServiceImplBase<TRequest, TResult>
    where TDbContext : DbContext
{
    private readonly IHttpContextAccessor _accessor;
    private readonly List<Func<bool>> _filters = new();
    private Func<TRequest> _setParameter;
    private Action<TResult> _executed;
    private Func<AbstractValidator<TRequest>> _validator;

    public ServiceExecutor(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public ServiceExecutor<TDbContext, TService, TRequest, TResult> SetValidator(Func<AbstractValidator<TRequest>> func)
    {
        _validator = func;
        return this;
    }

    public ServiceExecutor<TDbContext, TService, TRequest, TResult> AddFilter(Func<bool> filter)
    {
        // Logic for adding filter
        _filters.Add(filter);
        return this;
    }

    public ServiceExecutor<TDbContext, TService, TRequest, TResult> SetParameter(Func<TRequest> parameter)
    {
        // Logic for setting parameter
        _setParameter = parameter;
        return this;
    }
    
    public ServiceExecutor<TDbContext, TService, TRequest, TResult> Executed(Action<TResult> executed)
    {
        // Logic for setting executed action
        _executed = executed;
        return this;
    }
    
    public async Task ExecuteAsync()
    {
        foreach (var filter in _filters.xToList())
        {
            var filtered = filter.Invoke();
            if(filtered.xIsFalse()) return;
        }

        if (_setParameter.xIsEmpty()) throw new NotImplementedException("set parameter method not define");
        var parameter = _setParameter.Invoke();

        if (_validator.xIsNotEmpty())
        {
            var validator= _validator.Invoke();
            var valid = await validator.ValidateAsync(parameter);
            if (valid.IsValid.xIsFalse()) throw new Exception(valid.Errors.xJoin());
        }
        
        var db = _accessor.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
        var service = _accessor.HttpContext!.RequestServices.GetRequiredService<TService>();
        var session = _accessor.HttpContext!.RequestServices.GetRequiredService<ISessionContext>();
        service.Request = parameter;
        var executing = await service.OnExecutingAsync(db, session);
        if (executing.xIsTrue())
        {
            await service.OnExecuteAsync(db, session);    
        }
        _executed.Invoke(service.Result);
        db.ChangeTracker.Clear();
    }
}