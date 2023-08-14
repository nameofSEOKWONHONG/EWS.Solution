using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity;

/// <summary>
/// 년도 테이블이 아닐경우 0, 0, 0으로 설정함.
/// </summary>
[Table("Sequentials", Schema = "system")]
public class Sequential : EntityBase
{
    public string TableName { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Seq { get; set; }

    public static void Builder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sequential>().ToTable("Sequentials", "system");
        modelBuilder.Entity<Sequential>()
            .HasKey(e => new { e.TableName, e.Year, e.Month, e.Day, e.Seq });

        modelBuilder.Entity<Sequential>()
            .Property(e => e.TableName)
            .HasColumnName(nameof(TableName))
            .HasMaxLength(300)
            .IsRequired()
            .HasColumnOrder(1);

        modelBuilder.Entity<Sequential>()
            .Property(e => e.Year)
            .HasColumnName(nameof(Year))
            .IsRequired()
            .HasColumnOrder(2);

        modelBuilder.Entity<Sequential>()
            .Property(e => e.Month)
            .HasColumnName(nameof(Month))
            .IsRequired()
            .HasColumnOrder(3);

        modelBuilder.Entity<Sequential>()
            .Property(e => e.Day)
            .HasColumnName(nameof(Day))
            .IsRequired()
            .HasColumnOrder(4);
        
        modelBuilder.Entity<Sequential>()
            .Property(e => e.Seq)
            .HasColumnName(nameof(Seq))
            .IsRequired()
            .HasColumnOrder(5);
    }
}