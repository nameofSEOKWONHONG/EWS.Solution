using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace EWS.Domain.Common.Enums;

[JsonConverter(typeof(SmartEnumNameConverter<ENUM_WEATHER_FORECAST_TYPE,int>))]
public sealed class ENUM_WEATHER_FORECAST_TYPE : SmartEnum<ENUM_WEATHER_FORECAST_TYPE, int>
{
    public static readonly ENUM_WEATHER_FORECAST_TYPE Hot = new ENUM_WEATHER_FORECAST_TYPE(nameof(Hot), 0);
    public static readonly ENUM_WEATHER_FORECAST_TYPE Cool = new ENUM_WEATHER_FORECAST_TYPE(nameof(Cool), 1);
    public static readonly ENUM_WEATHER_FORECAST_TYPE Warm = new ENUM_WEATHER_FORECAST_TYPE(nameof(Warm),2);
    public ENUM_WEATHER_FORECAST_TYPE(string name, int value) : base(name, value)
    {
    }
}