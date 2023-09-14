// using EWS.Application.Service.Abstract;
// using Microsoft.AspNetCore.Http;
// using Microsoft.EntityFrameworkCore;
//
// namespace EWS.Infrastructure.ServiceRouter.Abstract;
//
// public abstract class SingletonServiceImpl : ISingletonService
// {
//     
// }
//
// public abstract class SingletonServiceImpl<TSelf, TRequest, TResult> : ServiceImplBase<TSelf>, IServiceImplBase<TRequest, TResult>, ISingletonService
// {
//     public TRequest Request { get; set; }
//     public TResult Result { get; set; }
//     public DbContext DbContext { get; set; }
//
//     protected SingletonServiceImpl(IHttpContextAccessor accessor) : base(accessor)
//     {
//         
//     }
// }