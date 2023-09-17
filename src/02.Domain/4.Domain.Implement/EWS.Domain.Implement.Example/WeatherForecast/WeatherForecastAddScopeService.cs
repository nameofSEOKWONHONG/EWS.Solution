using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Domain.Infra.QueryBuilder.NumberEntityBase;
using EWS.Domain.Infrastructure;
using EWS.Entity.Db;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastAddService : ServiceImplBase<WeatherForecastAddService, Entity.Example.WeatherForecast, IResultBase<int>>, IWeatherForecastAddService
{
    private readonly IWeatherForecastAdd2Service _service;
    public WeatherForecastAddService(EWSMsDbContext dbContext, ISessionContext context,
        IWeatherForecastAdd2Service service) : base(dbContext, context)
    {
        _service = service;
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

        IResultBase<int> tResult = null;
        await _service.Create<IWeatherForecastAdd2Service, Entity.Example.WeatherForecast, IResultBase<int>>()
            .UseTransaction(this.Db)
            .SetParameter(() => new Entity.Example.WeatherForecast()
            {
                City = this.Request.City,
                Date = DateTime.Now,
                TemperatureC = 10,
                Summary = "test2",
                WeatherForecastType = 1,
                CreatedBy = "system",
                CreatedOn = DateTime.Now,
                CreatedName = "system"
            })
            .OnExecuted((res) => tResult = res);

        this.Result = await JResult<int>.SuccessAsync(result.Id);
    }
}

public class WeatherForecastAdd2Service : ServiceImplBase<WeatherForecastAdd2Service, Entity.Example.WeatherForecast, IResultBase<int>>, IWeatherForecastAdd2Service
{
    public WeatherForecastAdd2Service(EWSMsDbContext dbContext, ISessionContext context) : base(dbContext, context)
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