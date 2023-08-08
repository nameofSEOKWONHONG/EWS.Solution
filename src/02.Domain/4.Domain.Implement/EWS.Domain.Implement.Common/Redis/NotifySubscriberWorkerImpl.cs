using EWS.Domain.Example;
using EWS.Domain.Infra.Abstract;
using EWS.Domain.Infra.Hubs;
using EWS.Domain.Infra.Redis;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EWS.Domain.Implement.Common.Redis;

public class NotifySubscriberWorkerImpl : RedisSubscriberWorkerImplBase
{
    public NotifySubscriberWorkerImpl(IServiceProvider serviceProvider, IConfiguration configuration, ISessionContext context, RedisWorkerProtocol redisWorkerProtocol) : base(serviceProvider, configuration, context, redisWorkerProtocol)
    {
    }

    public override async Task ExecuteAsync()
    {
        await using var scope = this.ServiceProvider.CreateAsyncScope();
        var redisHandler = scope.ServiceProvider.GetRequiredService<RedisHandler>();
        var exist = await redisHandler.GetAsync<RedisWorkerProtocol>(this.RedisWorkerProtocol.RedisKey);
        if (exist.xIsNotEmpty())
        {
            await redisHandler.RemoveAsync(this.RedisWorkerProtocol.RedisKey);    
        }
        
        //use websocket, notification to user.

        var hubConnection = this.ServiceProvider.GetRequiredService<NotificationHub>();
        await hubConnection.SendNotificationByConnectionId(exist.SenderConnectionId.xValue(""), "You have a new notification!");
        
        Log.Logger.Debug("Notification messages : {Msg}",RedisWorkerProtocol.xToJson());
    }
}