using System.Transactions;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Domain.Example;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastServiceV3: ScopeServiceImpl<WeatherForecastServiceV3, int, WeatherForecastResult>, IWeatherForecastServiceV3
{
    public WeatherForecastServiceV3(IHttpContextAccessor accessor) : base(accessor)
    {
        
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