using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastSingletonService : ServiceImplBase<WeatherForecastSingletonService, int, Entity.Example.WeatherForecast>, IWeatherForecastSingletonService
{
    private readonly Entity.Example.WeatherForecast _weatherForecast;
    public WeatherForecastSingletonService() : base()
    {
        _weatherForecast = new()
        {
            Id = 1000,
            City = "test",
            Date = DateTime.Now,
            Summary = "test",
            TenantId = "99999"
        };
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync()
    {
        this.Result = _weatherForecast;
        return Task.CompletedTask;
    }
}