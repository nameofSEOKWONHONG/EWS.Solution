using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Abstract;

public abstract class ServiceImplBase : IServiceImplBase
{
    protected readonly IHttpContextAccessor Accessor;
    protected Serilog.ILogger Logger => Serilog.Log.Logger;

    protected ServiceImplBase()
    {
        
    }

    protected ServiceImplBase(IHttpContextAccessor accessor)
    {
        Accessor = accessor;
    }
    
    public abstract Task<bool> OnExecutingAsync();

    public abstract Task OnExecuteAsync();
}

public abstract class ServiceImplBase<TSelf> : ServiceImplBase
{
    protected ServiceImplBase() : base()
    {
        Logger.Debug("Create instance {Name}", typeof(TSelf).Name);
    }
}

public abstract class ServiceImplBase<TSelf, TRequest, TResult> : IServiceImplBase<TRequest, TResult>
{
    public abstract Task<bool> OnExecutingAsync();

    public abstract Task OnExecuteAsync();

    public TRequest Request { get; set; }
    public TResult Result { get; set; }
}