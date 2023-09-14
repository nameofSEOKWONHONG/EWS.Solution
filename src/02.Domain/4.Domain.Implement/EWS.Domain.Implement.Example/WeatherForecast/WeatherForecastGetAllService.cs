using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Domain.Common.Enums;
using EWS.Domain.Example;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Domain.Infrastructure;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastGetAllService : ServiceImplBase<WeatherForecastGetAllService, WeatherForecatGetAllRequest, JPaginatedResult<WeatherForecastResult>>, IWeatherForecastGetAllService
{
    private readonly IWeatherForecastGetService _weatherForecastGetService;
    public WeatherForecastGetAllService(DbContext dbContext, ISessionContext context,
        IWeatherForecastGetService weatherForecastGetService) : base(dbContext, context)
    {
        _weatherForecastGetService = weatherForecastGetService;
    }
    

    public override Task<bool> OnExecutingAsync()
    {
        
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        this.Result = await Db.CreateSelectBuilder<Entity.Example.WeatherForecast>(Context)
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
        await ServiceLoader<IWeatherForecastGetService, int, WeatherForecastResult>
            .Create(_weatherForecastGetService)
            .AddFilter(() => true)
            .SetParameter(() => this.Result.Data.First().Id)
            .OnExecuted((res, v) =>
            {
                result = res;
            });
    }
}