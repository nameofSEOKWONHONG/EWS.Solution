using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using EWS.BlazorShared.Setup;
using EWS.Domain.Base;
using eXtensionSharp;
using Microsoft.Extensions.Logging;

namespace EWS.BlazorShared.Base;

public abstract class JManagerBase : IManager
{
    protected readonly HttpClient Client;
    protected readonly JAuthenticationStateProvider AuthenticationState;

    protected JManagerBase(HttpClient client, JAuthenticationStateProvider authenticationState)
    {
        Client = client;
        AuthenticationState = authenticationState;
    }

    private async Task InitHeaderTokenAsync()
    {
        if (AuthenticationState.AceessToken.xIsEmpty())
        {
            await AuthenticationState.GetAuthenticationStateAsync();
        }
        
        var token = AuthenticationState.AceessToken;
        if (token.xIsNotEmpty())
        {
            if (Client.DefaultRequestHeaders.xIsEmpty())
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else if (Client.DefaultRequestHeaders?.Authorization?.Scheme != "Bearer")
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        
    }
    
    public virtual async Task<JPaginatedResult<T>> GetAll<T>(string url) where T : JDisplayRow
    {
        var res = await Client.GetAsync($"api/{url}");
        var result = await res.Content.ReadFromJsonAsync<JPaginatedResult<T>>();
        for (var i = 0; i < result.Data.Count; i++)
        {
            var row = result.Data[i];
            row.RowClass = ((i + 1) % 2) == 0 ? "second" : "first";
        }

        return result;
    }
    
    public virtual async Task<JPaginatedResult<T>> GetAll<T>(string url, JRequestBase request) where T : JDisplayRow
    {
        var res = await Client.PostAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<JPaginatedResult<T>>();
        for (var i = 0; i < result.Data.Count; i++)
        {
            var row = result.Data[i];
            row.RowClass = ((i + 1) % 2) == 0 ? "second" : "first";
        }

        return result;
    }    

    public virtual async Task<T> Get<T>(string url)
    {
        var res = await Client.GetAsync($"api/{url}");
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }
    
    public virtual async Task<T> Get<T>(string url, JRequestBase request)
    {
        var res = await Client.PostAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }

    public virtual async Task<T> Insert<T>(string url, JRequestBase request)
    {
        var res = await Client.PostAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }
    
    public virtual async Task<T> Update<T>(string url, JRequestBase request)
    {
        var res = await Client.PutAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }    
    
    public virtual async Task<T> Remove<T>(string url)
    {
        var res = await Client.DeleteAsync($"api/{url}");
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }
    
    public virtual async Task<T> Remove<T>(string url, JRequestBase request)
    {
        var res = await Client.DeleteAsync($"api/{url}");
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }
    
    public virtual async Task<T> Active<T>(string url, JRequestBase request)
    {
        var res = await Client.PatchAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }
    
    public virtual async Task<T> Batch<T>(string url, JRequestBase request)
    {
        var res = await Client.PostAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }
    
    public virtual async Task<T> Bulk<T>(string url, JRequestBase request)
    {
        var res = await Client.PostAsync($"api/{url}", new StringContent(request.xToJson(), Encoding.UTF8, "application/json"));
        var result = await res.Content.ReadFromJsonAsync<T>();
        return result;
    }

    public virtual void Dispose()
    {
        
    }
}