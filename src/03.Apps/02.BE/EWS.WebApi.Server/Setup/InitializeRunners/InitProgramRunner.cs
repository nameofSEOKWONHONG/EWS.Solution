using EWS.Application;
using EWS.Domain.Infra.Migrations;
using EWS.WebApi.Server.ApplicationInitializer.Tenant;
using Microsoft.EntityFrameworkCore;

namespace EWS.WebApi.Server.ApplicationInitializer;

/// <summary>
/// 
/// </summary>
public class InitProgramRunner
{
    private static readonly Dictionary<string, Func<IServiceProvider, IApplicationInitializeRunner>> _programStates = new()
    {
        // 정방향 마이그레이션 
        {
            "es-mig-up", (sp) => new MigrationUpExecuteService(sp.GetRequiredService<DbContext>())
        },
        // 역방향 마이그레이션
        {
            "es-mig-down", (sp) => new MigrationDownExecuteService(sp.GetRequiredService<DbContext>())
        },
        // Function, Procedure 등 기타 업데이트
        {
            "es-mig-etc", (sp) => new MigrationElseExecuteService(sp.GetRequiredService<DbContext>())
        },
        // 테넌트 생성
        {
            "es-init", (sp) => new TenantInitializeRunner(sp)
        }
    };

    private readonly string[] _args;
    private readonly IServiceProvider _services;
    
    private InitProgramRunner(string[] args, IServiceProvider services)
    {
        _args = args;
        _services = services;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Run()
    {
        if (_programStates.TryGetValue(_args[0], out var v))
        {
            var instance = v.Invoke(_services);
            if (instance.Filter(_args))
            {
                instance.Execute(_args);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static InitProgramRunner Create(string[] args, IServiceProvider services)
    {
        return new InitProgramRunner(args, services);
    }
}