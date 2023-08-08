using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity.Db;

public abstract class JDbContextBase<TDbContextOptions> : DbContext
    where TDbContextOptions : DbContextOptions
{
    protected JDbContextBase(TDbContextOptions options) : base(options)
    {
        
    }

    protected JDbContextBase(string conn) : base(GetOptions(conn))
    {
    }
    
    private static DbContextOptions GetOptions(string connectionString)
    {
        return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,6)");
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
        {
            property.SetColumnType("nvarchar(128)");
        }
        
        // get all composite keys (entity decorated by more than 1 [Key] attribute
        foreach (var entity in modelBuilder.Model.GetEntityTypes()
                     .Where(t =>
                         t.ClrType.GetProperties()
                             .Count(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute))) > 1))
        {
            // get the keys in the appropriate order
            var orderedKeys = entity.ClrType
                .GetProperties()
                .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)))
                .OrderBy(p =>
                    p.CustomAttributes.Single(x => x.AttributeType == typeof(ColumnAttribute))?
                        .NamedArguments?.Single(y => y.MemberName == nameof(ColumnAttribute.Order))
                        .TypedValue.Value ?? 0)
                .Select(x => x.Name)
                .ToArray();

            // apply the keys to the model builder
            modelBuilder.Entity(entity.ClrType).HasKey(orderedKeys);
        }
        
        base.OnModelCreating(modelBuilder);
    }
}