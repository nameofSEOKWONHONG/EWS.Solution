using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Example.WeatherForecast;

public class WeatherForecastSingletonService : SingletonServiceImpl<WeatherForecastSingletonService, int, Entity.Example.WeatherForecast>, IWeatherForecastSingletonService
{
    private readonly Entity.Example.WeatherForecast _weatherForecast;
    public WeatherForecastSingletonService(IHttpContextAccessor accessor) : base(accessor)
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

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync(ISessionContext context)
    {
        this.Result = _weatherForecast;
        return Task.CompletedTask;
    }
}