using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Example;
using EWS.Domain.Infra.Sql;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastBulkInsertService : ScopeServiceImpl<WeatherForecastBulkInsertService, IEnumerable<WeatherForecastBulkRequest>, IResultBase>, IWeatherForecastBulkInsertService
{
    public WeatherForecastBulkInsertService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        await dbContext.CreateSqlBulkBuilder<WeatherForecastBulkRequest>()
            .BulkInsertAsync<Entity.Example.WeatherForecast>(this.Request.ToArray());
    }
}