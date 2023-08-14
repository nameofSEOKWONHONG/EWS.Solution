using EWS.Entity.Base;
using EWS.Entity.Example;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity.Db;

public class EWSMsDbContext : JDbContextBase<DbContextOptions<EWSMsDbContext>>
{
    public EWSMsDbContext(DbContextOptions<EWSMsDbContext> options) : base(options)
    {
        
    }
    
    public EWSMsDbContext(string conn) : base(conn)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region [system]

        Tenant.Builder(modelBuilder);
        Sequential.Builder(modelBuilder);

        #endregion

        #region [identity]

        User.Builder(modelBuilder);
        UserRole.Builder(modelBuilder);
        Role.Builder(modelBuilder);
        RoleClaim.Builder(modelBuilder);        

        #endregion

        #region [example]

        WeatherForecast.Builder(modelBuilder);

        #endregion
        
        base.OnModelCreating(modelBuilder);
    }

    #region [system]

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Sequential> Sequentials { get; set; }

    #endregion

    #region [identity]

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }    

    #endregion

    #region [example]

    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    
    #endregion

    #region [business]

    public DbSet<Resource> Resources { get; set; }
    public DbSet<SubResource> SubResources { get; set; }

    #endregion
}