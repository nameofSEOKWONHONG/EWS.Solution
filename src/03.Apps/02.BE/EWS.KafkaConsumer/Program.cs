using EWS.Domain.Infra.Kafka;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
#if DEBUG
    .AddJsonFile("appsettings.Development.json", true, true)
#else
                .AddJsonFile("appsettings.json")
#endif
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSerilog();
        services.AddSingleton<ApacheKafkaConsumerCallback>();
        services.Configure<ApacheKafkaOption>(configuration.GetSection("ApacheKafkaOption"));
        services.AddHostedService<ApacheKafkaConsumerBackgroundService>();
    })
    ;

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
var host = builder.Build();

var callback = host.Services.GetRequiredService<ApacheKafkaConsumerCallback>();
callback.Callback = (item) =>
{
    Console.WriteLine(item.xToJson());
};

await host.RunAsync();