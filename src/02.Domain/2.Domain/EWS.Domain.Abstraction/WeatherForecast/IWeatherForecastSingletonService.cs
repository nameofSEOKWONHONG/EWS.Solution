using EWS.Application.Service.Abstract;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastSingletonService : IServiceImplBase<int, Entity.Example.WeatherForecast>, ISingletonService
{
    
}