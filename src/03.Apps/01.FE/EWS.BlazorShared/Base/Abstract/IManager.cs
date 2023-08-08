using EWS.Domain.Base;

namespace EWS.BlazorShared.Base;

public interface IManager : IDisposable
{
    /// <summary>
    /// 다건 조회, Method : GET
    /// </summary>
    /// <param name="url"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<JPaginatedResult<T>> GetAll<T>(string url) where T : JDisplayRow;
    
    /// <summary>
    /// 다건 조회, Method : POST
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<JPaginatedResult<T>> GetAll<T>(string url, JRequestBase request) where T : JDisplayRow;

    /// <summary>
    /// 단건 조회, Method : GET
    /// </summary>
    /// <param name="url"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Get<T>(string url);
    
    /// <summary>
    /// 단건 조회, Method: POST
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Get<T>(string url, JRequestBase request);

    /// <summary>
    /// 단건 입력, Method : POST
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Insert<T>(string url, JRequestBase request);
    
    /// <summary>
    /// 단,다건 수정, Method : PUT
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Update<T>(string url, JRequestBase request);
    
    /// <summary>
    /// 단건 삭제, Method : DELETE
    /// </summary>
    /// <param name="url"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Remove<T>(string url);
    
    /// <summary>
    /// 다건 삭제, Method : DELETE
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Remove<T>(string url, JRequestBase request);
    
    /// <summary>
    /// 다건 비활성, Method : PATCH
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Active<T>(string url, JRequestBase request);
    
    /// <summary>
    /// 다건 입력, Method : POST
    /// 2000건 제한
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> Bulk<T>(string url, JRequestBase request);
}

public interface ITransientManager : IManager
{
    
}

public interface IScopeManager : IManager
{
    
}

public interface ISingletonManager : IManager
{
    
}