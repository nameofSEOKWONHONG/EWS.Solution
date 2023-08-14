using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Common.Enums;
using EWS.Domain.Example;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Domain.Infra.Redis;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastServiceV2: ScopeServiceImpl<WeatherForecastServiceV2, string, WeatherForecastResult>, IWeatherForecastServiceV2
{
    private readonly RedisHandler _redisHandler;
    public WeatherForecastServiceV2(IHttpContextAccessor accessor,
        RedisHandler redisHandler) : base(accessor)
    {
        this._redisHandler = redisHandler;
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        this.Result = await this.DbContext.CreateSelectBuilder<Entity.Example.WeatherForecast>(context)
            .SetQueryable(query =>
            {
                if (this.Request.xIsNotEmpty())
                {
                    query = query.Where(m => m.Id == 2);
                }

                return query;
            })
            .ToFirstAsync<WeatherForecastResult>(res => new WeatherForecastResult
            {
                Id = res.Id,
                Date = res.Date,
                Summary = res.Summary,
                TemperatureC = res.TemperatureC
            });
        
        if (this.Result.xIsNotEmpty())
        {
            var protocol = new RedisWorkerProtocol(context.TenantId, ENUM_REDIS_SUBSCRIBER_TYPE.Export, "hello")
            {
                Ip = XEnvExtensions.GetLocalIPAddress(),
                MachineName = Environment.MachineName,
                InstanceName = context.Configuration["Redis:InstanceName"].xValue(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")), //Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") //azure app service instance name
                SenderConnectionId = this.Request,
                Request = this.Result.xToJson()
            };
                
            await _redisHandler.SetAndPublishAsync("hello", protocol);
        }        
    }
}