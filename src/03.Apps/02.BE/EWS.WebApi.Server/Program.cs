using EWS.Application.Const;
using EWS.Domain.Abstraction.Common;
using EWS.Domain.Implement.Common.Redis.Subscribe;
using EWS.Domain.Infra.Hubs;
using EWS.Domain.Infra.Redis;
using EWS.Domain.Infra.Service;
using EWS.WebApi.Server;
using EWS.WebApi.Server.ApplicationInitializer;
using EWS.WebApi.Server.ApplicationInitializer.Tenant;
using eXtensionSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.vAddDatabase()
    .vAddSessionContext()
    .vAddLocalizer()
    .vAddIdentity(builder.Configuration)
    .vAddSwagger()
    .vAddCompression()
    .vAddNode()
    .vAddScrutor()
    .vAddHangfire(builder.Configuration)
    .vAddRedis(builder.Configuration)
    .vAddCors()
    .vAddTenantInit()
    .AddHttpContextAccessor()
    .AddAuthorizationCore()
    .AddSingleton<NotificationHub>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Host.vUseSerilog();

var app = builder.Build();
app.vUseConfigure()
    .vUseHangfire()
    .vUseBlazorHost();

#region [redis subscribe]
var redisSubscriber = app.Services.GetRequiredService<RedisSubscriber>();
IRedisSubscriberCallback subscriberImpl = new RedisSubscriberCallback(app.Services, app.Configuration);
await redisSubscriber.SubscribeToChannelAsync(ApplicationConstants.Redis.MessageChannel, subscriberImpl.Callback);
#endregion

var host = app.Services.GetRequiredService<IHostNotificationService>();
await host.NotificationAsync();

if (args.xIsNotEmpty() && args.xContains(new string[]{"run", "es-init" }))
{
    Console.WriteLine("Program initialize mode, Press enter");
    Console.ReadLine();
    using var scope = app.Services.CreateScope();
    InitProgramRunner.Create(args, scope.ServiceProvider).Run();
}
else
{
    app.Run();   
}