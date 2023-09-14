using EWS.Application.Service.Abstract;
using EWS.Domain.Example;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastServiceV3 : IServiceImplBase<int, WeatherForecastResult>, IScopeService
{
    
}