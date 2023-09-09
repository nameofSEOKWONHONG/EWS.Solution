using System.Data;
using System.Data.Common;
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
    // public static ServiceRouter<TDbContext> Create<TDbContext>(IHttpContextAccessor accessor,TransactionScopeOption transactionScopeOption)
    //     where TDbContext : DbContext
    // {
    //     return new ServiceRouter<TDbContext>(accessor, transactionScopeOption);
    // }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="isolationLevel"></param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static ServiceRouter<TDbContext> Create<TDbContext>(IHttpContextAccessor accessor,IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        where TDbContext : DbContext
    {
        return new ServiceRouter<TDbContext>(accessor, isolationLevel);
    }    
}

public sealed class ServiceRouter<TDbContext> : DisposeBase
    where TDbContext : DbContext
{
    private readonly TransactionScopeOption _transactionScopeOption;
    private readonly IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;
    private readonly List<Func<Task>> _registeredServices;
    private readonly IHttpContextAccessor _accessor;
    private readonly DbContext _dbContext;
    private IDbContextTransaction _dbContextTransaction = null;
    
    // public ServiceRouter(IHttpContextAccessor accessor, TransactionScopeOption transactionScopeOption)
    // {
    //     _accessor = accessor;
    //     _transactionScopeOption = transactionScopeOption;
    //     _registeredServices = new List<Func<Task>>();
    //     _dbContext = this._accessor.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
    // }
    
    public ServiceRouter(IHttpContextAccessor accessor, IsolationLevel isolationLevel)
    {
        _accessor = accessor;
        _isolationLevel = isolationLevel;
        _registeredServices = new List<Func<Task>>();
        _dbContext = this._accessor.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
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
        if (_dbContext.Database.CurrentTransaction.xIsEmpty())
        {   
            _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(_isolationLevel);
        }
        
        foreach (var service in _registeredServices)
        {
            try
            {
                await service.Invoke();
                if (_dbContextTransaction.xIsNotEmpty()) await _dbContextTransaction!.CommitAsync();
            }
            catch (Exception e)
            {
                if (_dbContextTransaction.xIsNotEmpty()) await _dbContextTransaction!.RollbackAsync();
                Log.Logger.Error(e, "Service Router Error : {Error}", e.Message);
                throw;
            }
            finally
            {
                _dbContext.ChangeTracker.Clear();
            }
        }
    }
}