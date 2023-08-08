using Jering.Javascript.NodeJS;

namespace EWS.Domain.Infra.Service;

public class NodeHostService : INodeHostService
{
    private readonly INodeJSService _nodeJsService;
    public NodeHostService(INodeJSService nodeJsService)
    {
        _nodeJsService = nodeJsService;
    }

    public async Task NpmVoidRun(string javascript)
    {
        await _nodeJsService.InvokeFromStringAsync(javascript);
    }

    public async Task<T> NpmRun<T>(string javascript)
    {
        return await _nodeJsService.InvokeFromStringAsync<T>(javascript);
    }
}