using EWS.Domain.Infra.Extension;
using EWS.Entity.Db;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

/// <summary>
/// 
/// </summary>
public class TenantCreator
{
    private readonly EWSMsDbContext _dbContext;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public TenantCreator(EWSMsDbContext dbContext)
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
        
        var rootTenant = _dbContext.Tenants.SingleOrDefault(m => m.Id == tenant.Id);
        if (rootTenant.xIsEmpty())
        {
            rootTenant = new Entity.Base.Tenant(tenant.Id, tenant.Name, tenant.DomainName, tenant.Code, tenant.TimeZone);
            rootTenant.CreatedBy = "SYSTEM";
            rootTenant.CreatedOn = DateTime.UtcNow;
            _dbContext.Tenants.Add(rootTenant);
            _dbContext.SaveChanges();
            _dbContext.ChangeTracker.Clear();
        }
    }
}