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
    
    public abstract Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context);

    public abstract Task OnExecuteAsync(DbContext dbContext, ISessionContext context);
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