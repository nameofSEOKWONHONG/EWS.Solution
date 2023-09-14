using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Example;
using EWS.Domain.Infra.Sql;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastBulkInsertService : ServiceImplBase<WeatherForecastBulkInsertService, IEnumerable<WeatherForecastBulkRequest>, IResultBase>, IWeatherForecastBulkInsertService
{
    public WeatherForecastBulkInsertService(DbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        await Db.CreateSqlBulkBuilder<WeatherForecastBulkRequest>()
            .BulkInsertAsync<Entity.Example.WeatherForecast>(this.Request.ToArray());
    }
}