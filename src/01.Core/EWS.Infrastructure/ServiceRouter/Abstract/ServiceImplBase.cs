using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;

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
    
    public abstract Task<bool> OnExecutingAsync(ISessionContext context);

    public abstract Task OnExecuteAsync(ISessionContext context);
}

public abstract class ServiceImplBase<TSelf> : ServiceImplBase
{
    protected ServiceImplBase() : base()
    {
        Logger.Debug("Create instance {Name}", typeof(TSelf).Name);

    }

    protected ServiceImplBase(IHttpContextAccessor accessor) : base(accessor)
    {
        
    }
}