using System.Globalization;
using System.Reflection;
using BlazorDB;
using Blazored.LocalStorage;
using EWS.Application;
using EWS.Application.Const;
using EWS.Application.Language;
using EWS.BlazorWasm.Base;
using EWS.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace EWS.BlazorWasm.Setup;

public static class ServiceSetupExtensions
{
    public static IServiceCollection vAddWasmLocalStorage(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorageAsSingleton();
        services.AddSingleton<IStateHandler, JStateBrowserLocalStorageHandler>();
        return services;
    }

    public static IServiceCollection vAddWasmLocalization(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<ILocalizer, WasmLocalizer>();
        return services;
    }

    public static IServiceCollection vAddWasmCulture(this IServiceCollection services)
    {
        services.AddSingleton<IWasmCulture, WasmCulture>();
        return services;
    }

    public static IServiceCollection vAddWasmSession(this IServiceCollection services)
    {
        services.AddSingleton<WasmSessionProvider>();
        return services;
    }

    public static IServiceCollection vAddWasmAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<JAuthenticationStateProvider>();
        services.AddSingleton<AuthenticationStateProvider, JAuthenticationStateProvider>();
        services.AddTransient<IAuthentication, JAuthentication>();
        services.AddAuthorizationCore(RegisterPermissionClaims);
        return services;
    }
    
    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var prop in typeof(JPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
            {
                options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
            }
        }
    }

    public static IServiceCollection vAddWasmHttp(this IServiceCollection services, WebAssemblyHostBuilder builder)
    {
        services.AddSingleton<IHttpInterceptor, JHttpInterceptor>();
        services.AddSingleton<JAuthenticationHeaderHandler>();
        services.AddSingleton(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(AppConstants.WasmHost.HTTP_CLIENT_NAME)
                .EnableIntercept(sp)
            )
            .AddHttpClient(AppConstants.WasmHost.HTTP_CLIENT_NAME, client =>
            {
                var culture = CultureInfo.DefaultThreadCurrentCulture;
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd($"{culture.ToString()},{culture.Name}");
                //client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                client.BaseAddress = new Uri("https://localhost:7201");

            })
            .AddHttpMessageHandler<JAuthenticationHeaderHandler>();
        services.AddHttpClientInterceptor();

        return services;
    }

    public static IServiceCollection vAddWasmIndexedDb(this IServiceCollection services)
    {
        services.AddBlazorDB(options =>
        {
            options.Name = AppConstants.WasmHost.INDEXED_DB_NAME;
            options.Version = 1;
            options.StoreSchemas = new List<StoreSchema>()
            {
                new StoreSchema()
                {
                    Name = AppConstants.WasmHost.INDEXED_DB_STORE_NAME,
                    PrimaryKey = "id",
                    PrimaryKeyAuto = true,
                    UniqueIndexes = new List<string> { "name" }
                }
            };
        });
        services.AddSingleton<IIndexDbHandler, JDbHandler>();
        return services;
    }

    public static IServiceCollection vAddWasmManager(this IServiceCollection services)
    {
        services.AddSingleton<ISpinLayoutService, SpinLayoutService>();
        services.Scan(scan => scan
            .FromAssemblies(
                Assembly.Load(AppConstants.WasmHost.CLIENT_NAME))
            .AddClasses(cls => cls.AssignableTo<IScopeManager>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(cls => cls.AssignableTo<ITransientManager>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(cls => cls.AssignableTo<ISingletonManager>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );
        return services;
    }

    public static IServiceCollection vAddWasmSignalR(this IServiceCollection services)
    {
        services.AddScoped<HubConnection>(sp => new HubConnectionBuilder()
            .WithUrl($"{AppConstants.WasmHost.URL}/{AppConstants.WasmHost.NOTIFICATION_HUB_NAME}")
            .WithAutomaticReconnect()
            .Build());
        return services;
    }
}