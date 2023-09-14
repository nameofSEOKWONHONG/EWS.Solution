// using System.Transactions;
// using EWS.Application;
// using EWS.Infrastructure.ServiceRouter.Abstract;
// using EWS.Infrastructure.Session.Abstract;
// using Microsoft.EntityFrameworkCore;
//
// namespace EWS.Infrastructure.ServiceRouter.Implement.Routers;
//
// public sealed class AppServiceRouter
// {
//     /// <summary>
//     /// Controller이외에 호출되는 ServiceRouter, 직접 DB 선언이 필요하고 ISessionContext는 UnverifiedSessionContext를 사용해야 한다.
//     /// </summary>
//     /// <param name="dbContext"></param>
//     /// <param name="ctx"></param>
//     /// <param name="transactionScopeOption"></param>
//     /// <typeparam name="TDbContext"></typeparam>
//     /// <returns></returns>
//     public static AppServiceRouter<TDbContext> Create<TDbContext>(TDbContext dbContext, ISessionContext ctx,
//         TransactionScopeOption transactionScopeOption)
//         where TDbContext : DbContext
//     {
//         return new AppServiceRouter<TDbContext>(dbContext, ctx, TransactionScopeOption.Required);
//     }
// }
//
// public sealed class AppServiceRouter<TDbContext> : DisposeBase
//     where TDbContext : DbContext
// {
//     private readonly TransactionScopeOption _transactionScopeOption;
//     private readonly List<Func<Task>> _registeredServices;
//
//     private readonly TDbContext _dbContext;
//     private readonly ISessionContext _currentSessionContext;
//     
//     public AppServiceRouter(TDbContext context, ISessionContext currentSessionContext, TransactionScopeOption transactionScopeOption)
//     {
//         _dbContext = context;
//         _currentSessionContext = currentSessionContext;
//         
//         _transactionScopeOption = transactionScopeOption;
//         _registeredServices = new List<Func<Task>>();
//     }
//     
//     public SessionServiceExecutor<TDbContext, TService, TRequest, TResult> Register<TService, TRequest, TResult>(TService service)
//         where TService : IServiceImplBase<TRequest, TResult>
//     {
//         var serviceExecutor = new SessionServiceExecutor<TDbContext, TService, TRequest, TResult>(_dbContext, _currentSessionContext, service);
//         _registeredServices.Add(serviceExecutor.ExecuteAsync);
//         return serviceExecutor;
//     }
//     
//     public async Task ExecuteAsync()
//     {
//         foreach (var service in _registeredServices)
//         {
//             using var transactionScope = new TransactionScope(_transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled);
//             await service.Invoke();
//             transactionScope.Complete();
//         }
//     }
// }