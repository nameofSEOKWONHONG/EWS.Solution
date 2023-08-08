using EWS.Application.Wrapper;
using EWS.BlazorWasm.Base;
using EWS.Domain.Base;

namespace EWS.BlazorWasm.Features.FetchData;

public class FetchDataManager : JManagerBase, IFetchDataManager, IScopeManager
{
    public FetchDataManager(HttpClient httpClient, ILogger<FetchDataManager> logger) : base(httpClient, logger)
    {
        Logger.LogDebug("MANAGER : CREATE");
    }

    public Task Write()
    {
        Logger.LogDebug("MANAGER : TEST");
        return Task.CompletedTask;
    }


    public override void Dispose()
    {
        Logger.LogDebug("MANAGER : DISPOSE");
    }

    public async Task<IResultBase> CallWorker(string connectionId)
    {
        var result = await this.Get<JResult>($"WeatherForecast/CallWorker?{nameof(connectionId)}={connectionId}");
        return result;
    }
}