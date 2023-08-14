using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastAddService : ScopeServiceImpl<WeatherForecastAddService, Entity.Example.WeatherForecast, IResultBase<int>>, IWeatherForecastAddService
{
    public WeatherForecastAddService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override async Task<bool> OnExecutingAsync(ISessionContext context)
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

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var result = await this.DbContext.CreateUpsertBuilder<Entity.Example.WeatherForecast>(context)
            .SetQueryable(query => query.Where(m => m.Id == this.Request.Id))
            .OnAddAsync(() => Task.FromResult(this.Request))
            .ExecuteAsync();

        this.Result = await JResult<int>.SuccessAsync(result.Id);
    }
}