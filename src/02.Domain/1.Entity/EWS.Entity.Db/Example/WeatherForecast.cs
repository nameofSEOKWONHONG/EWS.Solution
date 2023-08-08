using System.ComponentModel.DataAnnotations.Schema;
using EWS.Entity.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity.Db.Example;

/// <summary>
/// 비 제네릭 타입 선언시 예제
/// </summary>
[Table("WeatherForecasts", Schema = "example")]
public class WeatherForecast : NumberEntityBase
{
    public string City { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    /// <summary>
    /// allow null로 설정되어 있으나 Validator에서 필수로 구현함.
    /// WeatherForecastValidator 참조
    /// </summary>
    public string Summary { get; set; }
    
    public int WeatherForecastType { get; set; }

    public static void Builder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>().ToTable("WeatherForecasts", "example");
        modelBuilder.Entity<WeatherForecast>()
            .HasKey(e => new { e.TenantId, e.Id });

        modelBuilder.Entity<WeatherForecast>()
            .Property(e => e.City)
            .HasColumnName("City")
            .IsRequired()
            .HasColumnOrder(2);

        modelBuilder.Entity<WeatherForecast>()
            .Property(e => e.Date)
            .HasColumnName("Date")
            .IsRequired()
            .HasColumnOrder(3);

        modelBuilder.Entity<WeatherForecast>()
            .Property(e => e.TemperatureC)
            .HasColumnName("TemperatureC")
            .IsRequired()
            .HasColumnOrder(4);

        modelBuilder.Entity<WeatherForecast>()
            .Property(e => e.Summary)
            .HasColumnName("Summary")
            .HasColumnOrder(5);

        modelBuilder.Entity<WeatherForecast>()
            .Property(e => e.WeatherForecastType)
            .HasColumnName("WeatherForecastType")
            .HasColumnOrder(6);
    }

    public class Validator : AbstractValidator<WeatherForecast>
    {
        public Validator()
        {
            RuleFor(m => m.City).NotEmpty();
        }
    }
}