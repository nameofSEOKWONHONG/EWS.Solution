using EWS.Entity.Db;

namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

/// <summary>
/// 
/// </summary>
public class RoleClaimCreator
{
    private readonly EWSMsDbContext _dbContext;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public RoleClaimCreator(EWSMsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    public void Execute(string[] args)
    {
        var tenant = TenantParser.Create().Parse(args);
        var roles = _dbContext.Roles.Where(m => m.TenantId == tenant.Id).ToList();
        foreach (var role in roles)
        {
            var list = _dbContext.RoleClaims.Where(m => m.RoleId == role.Id).ToList();
            foreach (var item in list)
            {
                item.TenantId = tenant.Id;
                item.CreatedBy = "SYSTEM";
                item.CreatedOn = DateTime.UtcNow;
                _dbContext.RoleClaims.Update(item);
                _dbContext.SaveChanges();
            }
        }
    }
}