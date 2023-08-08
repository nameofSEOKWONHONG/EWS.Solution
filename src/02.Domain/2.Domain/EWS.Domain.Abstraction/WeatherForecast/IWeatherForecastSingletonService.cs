using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastSingletonService : IServiceImplBase<int, Entity.Db.Example.WeatherForecast>
{
    
}