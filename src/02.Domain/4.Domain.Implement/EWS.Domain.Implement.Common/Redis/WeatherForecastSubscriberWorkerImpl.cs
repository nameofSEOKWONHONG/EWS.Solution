using EWS.Domain.Abstraction.Common;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Common.Enums;
using EWS.Domain.Example;
using EWS.Domain.Infra.Abstract;
using EWS.Domain.Infra.Redis;
using EWS.Domain.Infrastructure;
using EWS.Entity.Db;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EWS.Domain.Implement.Common.Redis;

public class WeatherForecastSubscriberWorkerImpl : RedisSubscriberWorkerImplBase
{
    public WeatherForecastSubscriberWorkerImpl(IServiceProvider serviceProvider, IConfiguration configuration, ISessionContext context, RedisWorkerProtocol redisWorkerProtocol) 
        : base(serviceProvider, configuration, context, redisWorkerProtocol)
    {
        
    }
    
    public override async Task ExecuteAsync()
    {
        await using var scope = this.ServiceProvider.CreateAsyncScope();
        var redisHandler = scope.ServiceProvider.GetRequiredService<RedisHandler>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EWSMsDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IWeatherForecastServiceV3>();
        var hostNotificationService = scope.ServiceProvider.GetRequiredService<IHostNotificationService>();

        WeatherForecastResult result = null;
        
        var req = this.RedisWorkerProtocol.Request.xToEntity<WeatherForecastResult>();
        Log.Logger.Debug("Debug:{REQ}", req);
        
        await service.Create<IWeatherForecastServiceV3, int, WeatherForecastResult>()
            .AddFilter(() => this.RedisWorkerProtocol.xIsNotEmpty())
            .SetParameter(() => req.Id)
            .OnExecuted((res) => result = res);
        
        await Task.Delay(10 * 1000);
        await hostNotificationService.NotificationAsync();
        
        //write excel file...
        //cloud upload...
        //get url...
        //write link address to protocal...
        //set notification work end;
        await redisHandler.SetAndPublishAsync(ENUM_REDIS_SUBSCRIBER_TYPE.End.Name,
            new RedisWorkerProtocol(this.SessionContext.TenantId, ENUM_REDIS_SUBSCRIBER_TYPE.End, RedisWorkerProtocol.RedisKey)
            {
                Ip = XEnvExtensions.GetLocalIPAddress(),
                MachineName = Environment.MachineName,
                InstanceName = this.Configuration["Redis:InstanceName"].xValue(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")), //azure app service instance name
                Request = string.Empty
            });
        
        Log.Logger.Debug("Redis Subscriber retrieve data : {Data}", result.xToJson());
    }
}