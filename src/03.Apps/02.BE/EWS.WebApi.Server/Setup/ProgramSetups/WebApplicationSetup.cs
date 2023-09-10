using EWS.Application.Const;
using EWS.Domain.Abstraction.Common;
using EWS.Domain.Implement.Common.Redis.Subscribe;
using EWS.Domain.Infra.Redis;
using EWS.Domain.Infra.Service;
using Hangfire;

namespace EWS.WebApi.Server.Setup.ProgramSetups;

/// <summary>
/// 
/// </summary>
public static class WebApplicationSetup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication vUseHangfire(this WebApplication app)
    {
        #region [hangfire]

        app.UseHangfireDashboard();

#if !DEBUG
        var backgroundJobs = app.Services.GetRequiredService<IBackgroundJobClient>();
        var service = app.Services.GetRequiredService<IHostNofiticiationService>();
        backgroundJobs.Enqueue(() => service.Notification());        
#endif


        #endregion

        return app;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication vUseBlazorHost(this WebApplication app)
    {
        app.UseStaticFiles();
        app.UseBlazorFrameworkFiles();
        app.MapFallbackToFile("index.html");
        return app;
    }
}