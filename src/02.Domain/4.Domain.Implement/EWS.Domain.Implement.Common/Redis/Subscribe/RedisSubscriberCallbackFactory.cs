using EWS.Domain.Common.Enums;
using EWS.Domain.Example;
using EWS.Domain.Implement.Common.Redis;
using EWS.Domain.Infra.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.Extensions.Configuration;

namespace EWS.Domain.Implement.Common.Redis.Subscribe;

public class RedisSubscriberCallbackFactory
{
    private readonly Dictionary<ENUM_REDIS_SUBSCRIBER_TYPE, Func<IServiceProvider, IConfiguration, ISessionContext, RedisWorkerProtocol, IRedisSubscriberWorkerImplBase>> _workerStatus
        = new()
        {
            {
                ENUM_REDIS_SUBSCRIBER_TYPE.Export, (sp, configuration, context, protocol) => new WeatherForecastSubscriberWorkerImpl(sp, configuration, context, protocol)
            },
            {
                ENUM_REDIS_SUBSCRIBER_TYPE.End, (sp, configuration, context, protocol) => new NotifySubscriberWorkerImpl(sp, configuration, context, protocol)
            },
        };

    private readonly ENUM_REDIS_SUBSCRIBER_TYPE _selectedWorkerType;
    
    private RedisSubscriberCallbackFactory(ENUM_REDIS_SUBSCRIBER_TYPE workerType)
    {
        _selectedWorkerType = workerType;
    }

    public Func<IServiceProvider, IConfiguration, ISessionContext, RedisWorkerProtocol, IRedisSubscriberWorkerImplBase> Get()
    {
        if (_workerStatus.TryGetValue(_selectedWorkerType,
                out Func<IServiceProvider, IConfiguration, ISessionContext, RedisWorkerProtocol, IRedisSubscriberWorkerImplBase>
                    func))
        {
            return func;
        }

        return null;
    } 
    
    public static RedisSubscriberCallbackFactory Create(ENUM_REDIS_SUBSCRIBER_TYPE workerType)
    {
        return new RedisSubscriberCallbackFactory(workerType);
    }
}

