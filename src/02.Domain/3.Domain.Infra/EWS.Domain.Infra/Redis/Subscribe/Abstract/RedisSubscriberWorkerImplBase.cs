using EWS.Domain.Example;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.Extensions.Configuration;

namespace EWS.Domain.Infra.Abstract;

public abstract class RedisSubscriberWorkerImplBase : IRedisSubscriberWorkerImplBase
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IConfiguration Configuration;
    protected readonly ISessionContext SessionContext;
    protected readonly RedisWorkerProtocol RedisWorkerProtocol;
    protected RedisSubscriberWorkerImplBase(IServiceProvider serviceProvider, IConfiguration configuration, ISessionContext context, RedisWorkerProtocol redisWorkerProtocol)
    {
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        SessionContext = context;
        RedisWorkerProtocol = redisWorkerProtocol;
    }

    public abstract Task ExecuteAsync();
}