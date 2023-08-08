using System.Transactions;
using EWS.Application;
using EWS.Infrastructure.ServiceRouter.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Implement.Routers;

public sealed class ServiceBatchRouter
{
    /// <summary>
    /// Controller에서 호출되는 Batch ServiceRouter
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="transactionScopeOption"></param>
    /// <param name="items"></param>
    /// <typeparam name="TEContext"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns></returns>
    public static ServiceBatchRouter<TEContext, TRequest, TResult> Create<TEContext, TRequest, TResult>(IHttpContextAccessor accessor,
        TransactionScopeOption transactionScopeOption, IEnumerable<TRequest> items)
        where TEContext : DbContext
    {
        return new ServiceBatchRouter<TEContext, TRequest, TResult>(accessor, transactionScopeOption, items);
    }
}

public sealed class ServiceBatchRouter<TContext, TRequest, TResult> : DisposeBase
    where TContext : DbContext
{
    private readonly TransactionScopeOption _transactionScopeOption;
    private readonly List<Func<TRequest, Task>> _registeredServices;
    private readonly IHttpContextAccessor _accessor;
    private readonly IEnumerable<TRequest> _items;
    
    public ServiceBatchRouter(IHttpContextAccessor accessor, TransactionScopeOption transactionScopeOption, IEnumerable<TRequest> items)
    {
        _accessor = accessor;
        _transactionScopeOption = transactionScopeOption;
        _registeredServices = new List<Func<TRequest, Task>>();
        _items = items;
    }

    public ServiceExecutorBatch<TContext, TService, TRequest, TResult> Register<TService>()
        where TService : IServiceImplBase<TRequest, TResult>
    {
        var serviceExecutor = new ServiceExecutorBatch<TContext, TService, TRequest, TResult>(_accessor);
        _registeredServices.Add(serviceExecutor.ExecuteAsync);
        return serviceExecutor;
    }

    public async Task ExecuteAsync()
    {
        using var transactionScope = new TransactionScope(_transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled);
        foreach (var service in _registeredServices)
        {
            foreach (var item in _items)
            {
                await service.Invoke(item);
            }
        }
        transactionScope.Complete();
    }
} 