using System.Reflection;
using EWS.BlazorShared;
using EWS.BlazorShared.Base;
using EWS.BlazorShared.Setup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace EWS.BlazorServer.Setup;

public static class ServiceCollectionSetup
{
    public static IServiceCollection vAddBlazorServiceSetup(this IServiceCollection services)
    {
        services.AddScoped<ProtectedLocalStorage>();
        services.AddScoped<IStateHandler, JStateServerLocalStorageHandler>();
        services.AddScoped<JAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider, 
            JAuthenticationStateProvider>();

        services.AddScoped<ISpinLayoutService, SpinLayoutService>();
        services.AddScoped<HubConnection>(sp => new HubConnectionBuilder()
            .WithUrl($"{AppConstants.ServerHost.URL}/{AppConstants.ServerHost.NOTIFICATION_HUB_NAME}")
            .WithAutomaticReconnect()
            .Build());
        
        // Add services to the container.
        services.AddScoped<JAuthenticationHeaderHandler>();
        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(AppConstants.ServerHost.HOST_NAME))
            .AddHttpClient(AppConstants.ServerHost.HOST_NAME, client =>
            {
                #if DEBUG
                client.BaseAddress = new Uri(AppConstants.ServerHost.URL);
                #endif
            })
            .AddHttpMessageHandler<JAuthenticationHeaderHandler>();
        
        services.Scan(scan => scan
            .FromAssemblies(
                Assembly.Load(AppConstants.BlazorHost.HOST_NAME))
            .AddClasses(cls => cls.AssignableTo<IScopeManager>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            // .AddClasses(cls => cls.AssignableTo<ISessionContext>())
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            .AddClasses(cls => cls.AssignableTo<ITransientManager>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(cls => cls.AssignableTo<ISingletonManager>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );
        
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddAntDesign();

        return services;

    }
}