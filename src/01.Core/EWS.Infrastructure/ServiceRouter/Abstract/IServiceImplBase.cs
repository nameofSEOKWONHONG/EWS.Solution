using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Abstract;

public interface IServiceImplBase
{
    Task<bool> OnExecutingAsync(ISessionContext context);
    Task OnExecuteAsync(ISessionContext context);    
}

public interface IServiceImplBase<TRequest, TResult> : IServiceImplBase
{
    TRequest Request { get; set; }
    TResult Result { get; set; }
    DbContext DbContext { get; set; }
}