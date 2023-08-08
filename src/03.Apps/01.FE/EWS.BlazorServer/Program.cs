using EWS.BlazorServer.Setup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.vAddBlazorServiceSetup();

var app = builder.Build();
app.vUseBlazorSetup();

app.Run();