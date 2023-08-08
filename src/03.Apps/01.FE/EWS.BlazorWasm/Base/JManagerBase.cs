using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using EWS.Domain.Base;
using eXtensionSharp;

namespace EWS.BlazorWasm.Base;

public abstract class JManagerBase : IManager
{
    protected readonly HttpClient Client;
    protected readonly ILogger Logger;

    protected JManagerBase(HttpClient client, ILogger logger)
    {
        Client = client;
        Logger = logger;
    }
    
    public virtual async Task<JPaginatedResult<T>> GetAll<T>(string url, JRequestBase request) where T : JDisplayRow
    {
        var res = await Client.GetAsync($"api/{url}?{GenerateParam(request)}");
        var result = await res.Content.ReadFromJsonAsync<JPaginatedResult<T>>();
        for (var i = 0; i < result.Data.Count; i++)
        {
            var row = result.Data[i];
            row.RowClass = ((i + 1) % 2) == 0 ? "second" : "first";
        }

        return result;
    }

    private string GenerateParam(JRequestBase requst)
    {
        if (requst.xIsEmpty()) return string.Empty;
        var result = new List<string>();
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(requst))
        {
            result.Add(property.Name + "=" + property.GetValue(requst));
        }

        return string.Join("&", result);
    }
    
    public virtual async Task<JPaginatedResult<T>> GetAllByPost<T>(string url, JRequestBase request) where T : JDisplayRow
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