using Blazored.LocalStorage;
using eXtensionSharp;

namespace EWS.BlazorShared.Base;

public class JStateServerLocalStorageHandler : IStateHandler
{
    private readonly ILocalStorageService _localStorage;

    public JStateServerLocalStorageHandler(ILocalStorageService localStorageService)
    {
        _localStorage = localStorageService;
    }
    
    public async Task SetStateAsync(string key, string value)
    {
        if (value.xIsEmpty()) return;
        await _localStorage.SetItemAsync(key, value);
    }

    public async Task<T> GetStateAsync<T>(string key)
    {
        var result = await _localStorage.GetItemAsync<T>(key);
        return result;
    }

    public async Task<string> GetStateAsync(string key)
    {
        var result = await _localStorage.GetItemAsync<string>(key);
        if (result.xIsEmpty()) return string.Empty;
        return result;
    }

    public async Task<string> GetStateAndRemoveAsync(string key)
    {
        var result = await this.GetStateAsync(key);
        await _localStorage.RemoveItemAsync(key);
        return result;
    }

    public async Task RemoveStateAsync(string key)
    {
        await _localStorage.RemoveItemAsync(key);
    }

    public async Task ClearAsync()
    {
        await _localStorage.ClearAsync();
    }
}