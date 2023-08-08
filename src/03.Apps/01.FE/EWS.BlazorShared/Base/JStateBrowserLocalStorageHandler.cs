using Blazored.LocalStorage;
using eXtensionSharp;

namespace EWS.BlazorShared.Base;



public class JStateBrowserLocalStorageHandler : IStateHandler
{
    private readonly ILocalStorageService _storage;
    public JStateBrowserLocalStorageHandler(ILocalStorageService storage)
    {
        _storage = storage;
    }
    
    public async Task SetStateAsync(string key, string value)
    {
        if (value.xIsEmpty()) return;
        await _storage.SetItemAsync(key, value);
    }
    
    public async Task<T> GetStateAsync<T>()
    {
        var result = await _storage.GetItemAsync<T>(typeof(T).Name);
        return result;
    }

    /// <summary>
    /// 제네릭 형태는 난독화 지원 안함.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<T> GetStateAndRemoveAsync<T>()
    {
        var result = await this.GetStateAsync<T>();
        await _storage.RemoveItemAsync(typeof(T).Name);
        return result;
    }

    public async Task<T> GetStateAsync<T>(string key)
    {
        var result = await _storage.GetItemAsync<T>(key);
        return result;
    }
    
    /// <returns></returns>
    public async Task<T> GetStateAndRemoveAsync<T>(string key)
    {
        var result = await this.GetStateAsync<T>(key);
        await _storage.RemoveItemAsync(typeof(T).Name);
        return result;
    }

    /// <summary>
    /// 기본 인코딩 제공, 난독화
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetStateAsync(string key)
    {
        var result = await _storage.GetItemAsync<string>(key);
        if (result.xIsEmpty()) return string.Empty;
        if (result.xIsEmpty()) return string.Empty;
        return result;
    }

    /// <summary>
    /// 기본 인코딩 제공, 난독화
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetStateAndRemoveAsync(string key)
    {
        var result = await this.GetStateAsync(key);
        await _storage.RemoveItemAsync(key);
        return result;
    }

    public async Task RemoveStateAsync(string key)
    {
        await _storage.RemoveItemAsync(key);
    }

    public async Task ClearAsync()
    {
        await _storage.ClearAsync();
    }
}