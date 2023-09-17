using EWS.Application.Service.Abstract;
using EWS.Application.Wrapper;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastAddService : IServiceImplBase<Entity.Example.WeatherForecast, IResultBase<int>>, IScopeService
{
    
}

public interface IWeatherForecastAdd2Service : IServiceImplBase<Entity.Example.WeatherForecast, IResultBase<int>>, IScopeService
{
    
}
