using System.ComponentModel.DataAnnotations.Schema;
using EWS.Domain.Base;

namespace EWS.Domain.Example;

[Table("WeatherForecasts", Schema = "example")]
public class WeatherForecastBulkRequest : JBulkBase
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public string City { get; set; }

    public int TemperatureC { get; set; }

    public string Summary { get; set; }
    
    public int WeatherForecastType { get; set; }
    
    
}