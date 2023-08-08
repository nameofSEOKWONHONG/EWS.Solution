using EWS.Application.Wrapper;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastAddService : IServiceImplBase<Entity.Db.Example.WeatherForecast, IResultBase<int>>
{
    
}
