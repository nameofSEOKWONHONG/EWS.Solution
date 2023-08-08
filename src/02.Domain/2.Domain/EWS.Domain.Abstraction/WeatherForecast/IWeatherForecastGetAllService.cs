using System;
using System.Collections.Generic;
using EWS.Domain.Base;
using EWS.Domain.Example;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.WeatherForecast;

public interface IWeatherForecastGetAllService : IServiceImplBase<WeatherForecatGetAllRequest, JPaginatedResult<WeatherForecastResult>>
{
    
}