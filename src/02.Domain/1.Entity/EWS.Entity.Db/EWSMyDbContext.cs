using Microsoft.EntityFrameworkCore;

namespace EWS.Entity.Db;

public class EWSMyDbContext : JDbContextBase<DbContextOptions<EWSMyDbContext>>
{
    public EWSMyDbContext(DbContextOptions<EWSMyDbContext> options) : base(options)
    {
    }

    public EWSMyDbContext(string conn) : base(conn)
    {
    }
}