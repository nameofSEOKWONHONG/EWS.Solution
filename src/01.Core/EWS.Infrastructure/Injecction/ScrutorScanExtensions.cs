using System.Reflection;
using EWS.Application.Service.Abstract;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Infrastructure.Injecction;

public static class ScrutorScanExtensions
{
    public static IServiceCollection AddScan(this IServiceCollection services, string domainName, string targetExtensionName)
    {
        var files = Directory.GetFiles(AppContext.BaseDirectory)
            .Where(m => Path.GetFileName(m).Contains(domainName) && Path.GetExtension(m) == targetExtensionName)
            .ToList();
        var assemblies = new List<Assembly>();
        files.xForEach(file =>
        {
            assemblies.Add(Assembly.Load(Path.GetFileNameWithoutExtension(file)));
        });
    
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(cls => cls.AssignableTo<IScopeService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            // .AddClasses(cls => cls.AssignableTo<ISessionContext>())
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            .AddClasses(cls => cls.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(cls => cls.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );

        return services;
    } 
}