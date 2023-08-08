using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Example;
using EWS.Domain.Infra.Sql;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastBulkInsertService : ScopeServiceImpl<WeatherForecastBulkInsertService, IEnumerable<WeatherForecastBulkRequest>, IResultBase>, IWeatherForecastBulkInsertService
{
    public WeatherForecastBulkInsertService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        await this.DbContext.CreateSqlBulkBuilder<WeatherForecastBulkRequest>(context)
            .BulkInsertAsync<Entity.Db.Example.WeatherForecast>(this.Request.ToArray());
    }
}