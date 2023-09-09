using System.Transactions;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Domain.Common.Enums;
using EWS.Domain.Example;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastGetAllService : ScopeServiceImpl<WeatherForecastGetAllService, WeatherForecatGetAllRequest, JPaginatedResult<WeatherForecastResult>>, IWeatherForecastGetAllService
{
    public WeatherForecastGetAllService(IHttpContextAccessor accessor) : base(accessor)
    {
        
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        this.Result = await this.DbContext.CreateSelectBuilder<Entity.Example.WeatherForecast>(context)
            .SetRequest(this.Request)
            .SetQueryable(query => query)
            .ToPaginationAsync<WeatherForecastResult>(res =>
            {
                var list = new List<WeatherForecastResult>();
                foreach (var weatherForecast in res.xToList())
                {
                    list.Add(new WeatherForecastResult
                    {
                        Id = weatherForecast.Id,
                        Date = weatherForecast.Date,
                        Summary = weatherForecast.Summary,
                        TemperatureC = weatherForecast.TemperatureC,
                        WeatherForecastType = ENUM_WEATHER_FORECAST_TYPE.FromValue(weatherForecast.WeatherForecastType)
                    });
                }

                return list;
            });
        
        WeatherForecastResult result = null;
        using var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor);
        var now = DateTime.Now;
        sr.Register<IWeatherForecastGetService, int, WeatherForecastResult>()
            .AddFilter(() => true)
            .SetParameter(() => this.Result.Data.First().Id)
            .Executed(res =>
            {
                result = res;
            });

        await sr.ExecuteAsync();
    }
}