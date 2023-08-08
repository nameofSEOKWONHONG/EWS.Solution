using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EWS.BlazorWasm;
using EWS.BlazorWasm.Base;
using EWS.BlazorWasm.Setup;
using eXtensionSharp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.vAddWasmLocalStorage()
    .vAddWasmLocalization()
    .vAddWasmCulture()
    .vAddWasmSession()
    .vAddWasmAuthentication()
    .vAddWasmHttp(builder)
    .vAddWasmIndexedDb()
    .vAddWasmManager()
    .vAddWasmSignalR()
    .AddAntDesign();

var host = builder.Build();
            
var state = host.Services.GetRequiredService<IStateHandler>();
var result = await state.GetStateAsync(AppConstants.WasmHost.CULTURE_NAME);
            
CultureInfo initCulture;
if (!result.xIsEmpty())
{
    initCulture = new CultureInfo(result);
}
else
{
    //default culture
    initCulture = new CultureInfo("en-US");
    await state.SetStateAsync(AppConstants.WasmHost.CULTURE_NAME, "en-US");
}

CultureInfo.DefaultThreadCurrentCulture = initCulture;
CultureInfo.DefaultThreadCurrentUICulture = initCulture;

try
{
    await host.RunAsync();
}
catch (Exception e)
{
    await Console.Error.WriteLineAsync(e.Message);
}