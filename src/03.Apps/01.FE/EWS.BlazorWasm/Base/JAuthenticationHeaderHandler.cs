#define __WASM__

using System.Net.Http.Headers;

namespace EWS.BlazorWasm.Base;

public class JAuthenticationHeaderHandler: DelegatingHandler
{
    private readonly IStateHandler _stateHandler;
    
    public JAuthenticationHeaderHandler(IStateHandler stateHandler)
    {
        this._stateHandler = stateHandler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        #if __WASM__
        if (request.Headers.Authorization?.Scheme != "Bearer")
        {
            var savedToken = await this._stateHandler.GetStateAsync(StorageConstants.Local.AuthToken);

            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            }
        }
        #endif
        
        var result = await base.SendAsync(request, cancellationToken);
        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
            result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.Unauthorized };
        }
        return result;
    }
}