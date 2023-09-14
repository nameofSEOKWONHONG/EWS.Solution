using System.Collections.Generic;
using EWS.Application.Service.Abstract;
using EWS.Application.Wrapper;
using EWS.Domain.Example;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastBulkInsertService : IServiceImplBase<IEnumerable<WeatherForecastBulkRequest>, IResultBase>, IScopeService
{
    
}