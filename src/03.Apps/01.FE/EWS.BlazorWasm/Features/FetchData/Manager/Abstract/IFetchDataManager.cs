using EWS.Application.Wrapper;
using EWS.BlazorWasm.Base;

namespace EWS.BlazorWasm.Features.FetchData;

public interface IFetchDataManager : IManager
{
    Task Write();
    Task<IResultBase> CallWorker(string connectionId);
}
