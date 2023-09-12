using EWS.Application.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Abstract;

public abstract class ScopeServiceImpl : IScopeService
{
    
}
    
public abstract class ScopeServiceImpl<TSelf, TRequest, TResult> : ServiceImplBase<TSelf>, IServiceImplBase<TRequest, TResult>, IScopeService
{
    public TRequest Request { get; set; }
    public TResult Result { get; set; }

    protected ScopeServiceImpl(IHttpContextAccessor accessor) : base(accessor)
    {
        
    }
}