using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Example;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastServiceV3: ServiceImplBase<WeatherForecastServiceV3, int, WeatherForecastResult>, IWeatherForecastServiceV3
{
    public WeatherForecastServiceV3(DbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        this.Result = await Db.CreateSelectBuilder<Entity.Example.WeatherForecast>(Context)
            .SetQueryable(query =>
            {
                if (this.Request > 0)
                {
                    query = query.Where(m => m.Id == this.Request);
                }

                return query;
            })
            .ToFirstAsync<WeatherForecastResult>(res => new WeatherForecastResult
            {
                Date = res.Date,
                Summary = res.Summary,
                TemperatureC = res.TemperatureC
            });
    }
}