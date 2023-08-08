using EWS.Domain.Base;

namespace EWS.BlazorWasm.Base;

public interface IIndexDbHandler
{
    Task InitializeAsync();

    Task SetAsync<T>(string name, T data, int timeout)
        where T : IDbStoreBase;

    Task<DbStoreSchemaBase<T>> GetAsync<T>(int id)
        where T : IDbStoreBase;

    Task<DbStoreSchemaBase<T>> GetByIdAsync<T>(int id)
        where T : IDbStoreBase;

    Task<DbStoreSchemaBase<T>> GetByNameAsync<T>(string name)
        where T : IDbStoreBase;

    Task RemoveAsync(string name);

    Task ClearAsync();
}