using EWS.Application;
using EWS.Entity.Db;
using EWS.WebApi.Server.ApplicationInitializer.Tenant;
using Microsoft.EntityFrameworkCore;

namespace EWS.WebApi.Server.ApplicationInitializer;

/// <summary>
/// 
/// </summary>
public class TenantInitializeRunner : IApplicationInitializeRunner
{
    private readonly IServiceProvider _service;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    public TenantInitializeRunner(IServiceProvider service)
    {
        _service = service;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Filter(string[] args)
    {
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Execute(string[] args)
    {
        _service.GetService<TenantCreator>().Execute(args);
        _service.GetService<RootCreator>().Execute(args);
        _service.GetService<AdminCreator>().Execute(args);
        _service.GetService<RoleClaimCreator>().Execute(args);
        
        //TODO : CREATE COMPANY, MENU, ETC...
    }
}