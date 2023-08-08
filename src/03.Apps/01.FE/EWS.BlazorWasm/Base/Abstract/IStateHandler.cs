namespace EWS.BlazorWasm.Base;

/// <summary>
/// localstorage를 이용한 상태 저장
/// </summary>
public interface IStateHandler
{
    //Task SetStateAsync<T>(T state);

    //Task SetStateAsync<T>(string key, T state);

    Task SetStateAsync(string key, string value);

    //Task<T> GetStateAsync<T>();

    /// <summary>
    /// 조회 후 삭제
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    //Task<T> GetStateAndRemoveAsync<T>();

    Task<T> GetStateAsync<T>(string key);

    /// <summary>
    /// 조회 후 삭제
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    //Task<T> GetStateAndRemoveAsync<T>(string key);

    Task<string> GetStateAsync(string key);

    /// <summary>
    /// 조회 후 삭제
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string> GetStateAndRemoveAsync(string key);

    Task RemoveStateAsync(string key);

    Task ClearAsync();
}