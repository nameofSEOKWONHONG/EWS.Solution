using EWS.Domain.Base;
using EWS.Domain.Common.Enums;

namespace EWS.Domain.Example;

public class WeatherForecastResult : JDisplayRow {
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; }
    
    public ENUM_WEATHER_FORECAST_TYPE WeatherForecastType { get; set; }
}