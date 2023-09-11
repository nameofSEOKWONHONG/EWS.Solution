using EWS.Domain.Infra.Hubs;
using EWS.Domain.Infra.Middleware;
using Serilog;

namespace EWS.WebApi.Server.Setup.ProgramSetups;

/// <summary>
/// 
/// </summary>
public static class HostBuilderSetup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder vUseSerilog(this IHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
#if DEBUG
            .AddJsonFile("appsettings.Development.json")
#else
                .AddJsonFile("appsettings.json")
#endif
            .AddEnvironmentVariables()
            .Build();
            
#if DEBUG   
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();            
#endif
            
#if !DEBUG 
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
#endif
        builder.UseSerilog();
        
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication vUseConfigure(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("AllowAllCors");
        
        app.Use((context, next)=>
        {
            if (context.Request.Method == HttpMethods.Post && context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();    
            }
            return next();
        });
        
        app.UseMiddleware<JErrorHandleMiddleware>();

        app.UseStaticFiles();
        
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.MapDefaultControllerRoute();
        
        app.UseResponseCompression();
        
        app.MapHub<NotificationHub>("/notificationHub");

        return app;
    }
}