using System.Transactions;
using EWS.Application;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using IsolationLevel = System.Data.IsolationLevel;

namespace EWS.Infrastructure.ServiceRouter.Implement.Routers;

public sealed class ServiceRouter
{
    /// <summary>
    /// Controller에서 호출되는 ServiceRouter
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="transactionScopeOption"></param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static ServiceRouter<TDbContext> Create<TDbContext>(IHttpContextAccessor accessor,TransactionScopeOption transactionScopeOption)
        where TDbContext : DbContext
    {
        return new ServiceRouter<TDbContext>(accessor, transactionScopeOption);
    }
}

public sealed class ServiceRouter<TDbContext> : DisposeBase
    where TDbContext : DbContext
{
    private readonly TransactionScopeOption _transactionScopeOption;
    private readonly List<Func<Task>> _registeredServices;
    private readonly IHttpContextAccessor _accessor;
    
    public ServiceRouter(IHttpContextAccessor accessor, TransactionScopeOption transactionScopeOption)
    {
        _accessor = accessor;
        _transactionScopeOption = transactionScopeOption;
        _registeredServices = new List<Func<Task>>();
    }
    
    public ServiceExecutor<TDbContext, TService, TRequest, TResult> Register<TService, TRequest, TResult>()
        where TService : IServiceImplBase<TRequest, TResult>
    {
        var serviceExecutor = new ServiceExecutor<TDbContext, TService, TRequest, TResult>(_accessor);
        _registeredServices.Add(serviceExecutor.ExecuteAsync);
        return serviceExecutor;
    }

    public SimpleServiceExecutor Register()
    {
        var serviceExecutor = new SimpleServiceExecutor(_accessor);
        _registeredServices.Add(serviceExecutor.ExecuteAsync);
        return serviceExecutor;
    }
    
    public async Task ExecuteAsync()
    {
        foreach (var service in _registeredServices)
        {
            var db = this._accessor.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
            
            IDbContextTransaction tran = null;
            if (db.Database.CurrentTransaction.xIsEmpty())
            {
                if (_transactionScopeOption == TransactionScopeOption.Required)
                {
                    tran = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, new CancellationToken()); 
                }
                else
                {
                    tran = await db.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, new CancellationToken());    
                }
            }
            
            try
            { 
                await service.Invoke();
                if (tran.xIsNotEmpty())
                {
                    await tran!.CommitAsync();    
                }
                db.ChangeTracker.Clear();
            }
            catch (Exception e)
            {
                if (tran.xIsNotEmpty())
                {
                    await tran!.RollbackAsync();    
                }
                Log.Logger.Error(e, "Service Router Error : {Error}", e.Message);
                throw;
            }
        }
    }
}





