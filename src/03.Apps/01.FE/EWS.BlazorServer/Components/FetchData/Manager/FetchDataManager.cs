using EWS.Application.Wrapper;
using EWS.BlazorShared.Base;
using EWS.BlazorShared.Setup;
using EWS.Domain.Base;
using Serilog;

namespace EWS.BlazorServer.Components.FetchData;

public class FetchDataManager : JManagerBase, IFetchDataManager, IScopeManager
{
    public FetchDataManager(HttpClient httpClient, JAuthenticationStateProvider authenticationStateProvider) : base(httpClient, authenticationStateProvider)
    {
        Log.Logger.Debug("MANAGER : CREATE");
    }

    public Task Write()
    {
        Log.Logger.Debug("MANAGER : TEST");
        return Task.CompletedTask;
    }


    public override void Dispose()
    {
        Log.Logger.Debug("MANAGER : DISPOSE");
    }

    public async Task<IResultBase> CallWorker(string connectionId)
    {
        var result = await this.Get<JResult>($"WeatherForecast/CallWorker?{nameof(connectionId)}={connectionId}");
        return result;
    }
}