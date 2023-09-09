using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Common.Enums;
using EWS.Domain.Example;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastGetService : ScopeServiceImpl<WeatherForecastGetService, int, WeatherForecastResult>, IWeatherForecastGetService
{
    public WeatherForecastGetService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(this.Request > 0);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        this.Result = await dbContext.CreateSelectBuilder<Entity.Example.WeatherForecast>(context)
            .SetQueryable(q => q.Where(m => m.Id == this.Request))
            .ToFirstAsync<WeatherForecastResult>(res => new WeatherForecastResult()
            {
                Date = res.Date,
                Summary = res.Summary,
                TemperatureC = res.TemperatureC,
                WeatherForecastType = ENUM_WEATHER_FORECAST_TYPE.FromValue(res.WeatherForecastType)
            });
    }
}