using EWS.Domain.Example;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastServiceV2 : IServiceImplBase<string, WeatherForecastResult>
{
    
}

