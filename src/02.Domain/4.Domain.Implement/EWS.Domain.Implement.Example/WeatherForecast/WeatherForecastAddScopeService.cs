using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastAddService : ServiceImplBase<WeatherForecastAddService, Entity.Example.WeatherForecast, IResultBase<int>>, IWeatherForecastAddService
{
    public WeatherForecastAddService(DbContext dbContext, ISessionContext context) : base(dbContext, context)
    {
    }

    public override async Task<bool> OnExecutingAsync()
    {
        var validator = new Entity.Example.WeatherForecast.Validator();
        var valid = validator.Validate(this.Request);
        if (valid.IsValid.xIsFalse())
        {
            this.Result = await JResult<int>.FailAsync(valid.Errors.Select(m => m.ErrorMessage).xJoin());
            return false;
        }

        return true;
    }

    public override async Task OnExecuteAsync()
    {
        var result = await Db.CreateUpsertBuilder<Entity.Example.WeatherForecast>(Context)
            .SetQueryable(query => query.Where(m => m.Id == this.Request.Id))
            .OnAddAsync(() => Task.FromResult(this.Request))
            .ExecuteAsync();

        this.Result = await JResult<int>.SuccessAsync(result.Id);
    }
}