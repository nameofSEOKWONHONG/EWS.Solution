using EWS.Application.Wrapper;
using EWS.BlazorShared.Base;

namespace EWS.BlazorServer.Components.FetchData;

public interface IFetchDataManager : IManager
{
    Task Write();
    Task<IResultBase> CallWorker(string connectionId);
}
