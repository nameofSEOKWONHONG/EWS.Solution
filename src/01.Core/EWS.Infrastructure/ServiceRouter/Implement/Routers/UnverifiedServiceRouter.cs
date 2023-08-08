using System.Transactions;
using EWS.Application;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Implement.Routers;

public sealed class UnverifiedServiceRouter
{
    /// <summary>
    /// Controller이외에 호출되는 ServiceRouter, 직접 DB 선언이 필요하고 ISessionContext는 UnverifiedSessionContext를 사용해야 한다.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="ctx"></param>
    /// <param name="transactionScopeOption"></param>
    /// <typeparam name="TEContext"></typeparam>
    /// <returns></returns>
    public static UnverifiedServiceRouter<TEContext> Create<TEContext>(TEContext dbContext, ISessionContext ctx,
        TransactionScopeOption transactionScopeOption)
        where TEContext : DbContext
    {
        return new UnverifiedServiceRouter<TEContext>(dbContext, ctx, TransactionScopeOption.Required);
    }
}

public sealed class UnverifiedServiceRouter<TContext> : DisposeBase
    where TContext : DbContext
{
    private readonly TransactionScopeOption _transactionScopeOption;
    private readonly List<Func<Task>> _registeredServices;

    private readonly TContext _dbContext;
    private readonly ISessionContext _currentSessionContext;
    
    public UnverifiedServiceRouter(TContext context, ISessionContext currentSessionContext, TransactionScopeOption transactionScopeOption)
    {
        _dbContext = context;
        _currentSessionContext = currentSessionContext;
        
        _transactionScopeOption = transactionScopeOption;
        _registeredServices = new List<Func<Task>>();
    }
    
    public SessionServiceExecutor<TContext, TService, TRequest, TResult> Register<TService, TRequest, TResult>(TService service)
        where TService : IServiceImplBase<TRequest, TResult>
    {
        var serviceExecutor = new SessionServiceExecutor<TContext, TService, TRequest, TResult>(_dbContext, _currentSessionContext, service);
        _registeredServices.Add(serviceExecutor.ExecuteAsync);
        return serviceExecutor;
    }
    
    public async Task ExecuteAsync()
    {
        foreach (var service in _registeredServices)
        {
            using var transactionScope = new TransactionScope(_transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled);
            await service.Invoke();
            transactionScope.Complete();
        }
    }
}