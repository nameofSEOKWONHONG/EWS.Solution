using EWS.Domain.Abstraction.Common;
using EWS.Domain.Example;
using EWS.Domain.Infra.Redis;
using EWS.Domain.Infra.Service;
using EWS.Domain.Infra.Session;
using EWS.Domain.Infra.Session.Accessor;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace EWS.Domain.Implement.Common.Redis.Subscribe;

public class RedisSubscriberCallback : IRedisSubscriberCallback
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    public RedisSubscriberCallback(IServiceProvider services, 
        IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public async Task Callback(RedisChannel channel, RedisValue value)
    {
        try
        {
            var key = (string)value;
            Log.Logger.Debug("Worker key is {Key}", key);
            
            await using var scope = this._services.CreateAsyncScope();
            var redisHandler = scope.ServiceProvider.GetRequiredService<RedisHandler>();
            var protocol = await redisHandler.GetAsync<RedisWorkerProtocol>(key);
            
            Log.Logger.Debug("Protocol : {Protocol}", protocol);
            
            if (protocol.Ip == XEnvExtensions.GetLocalIPAddress() &&
                protocol.MachineName == Environment.MachineName &&
                protocol.InstanceName == _configuration["Redis:InstanceName"].xValue(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"))
               )
            {
                UnverifiedContext unverifiedContext = new()
                {
                    TenantId = protocol.TenantId,
                    InfraAccessor = new InfraAccessor(new SequentialService())
                    //Add ...
                    
                };

                var func = RedisSubscriberCallbackFactory.Create(protocol.WorkerType).Get();
                if (func.xIsNotEmpty())
                {
                    var worker = func.Invoke(this._services, _configuration, unverifiedContext, protocol);
                    Log.Logger.Debug("RedisSubscriberCallbackFactory worker name {Name}", worker.GetType().Name);
                    await worker.ExecuteAsync();
                }
                else
                {
                    Log.Logger.Warning("worker not found : {Key}", key);
                }
                Log.Logger.Debug("redis subscriber work is end, gracefully : {Key}", key);
            }
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "IRedisSubscriberWorkerImplBase error : {Error}", e.Message);
        }
    }
}
