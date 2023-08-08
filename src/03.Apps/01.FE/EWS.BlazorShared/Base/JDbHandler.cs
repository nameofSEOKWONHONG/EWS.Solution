using BlazorDB;
using EWS.Application.Const;
using EWS.BlazorShared.Base.Abstract;
using EWS.Domain.Base;
using eXtensionSharp;

namespace EWS.BlazorShared.Base;

public class JDbHandler : IIndexDbHandler
{
    private readonly IBlazorDbFactory _dbFactory;
    private IndexedDbManager _dbManager;
    
        public JDbHandler(IBlazorDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task InitializeAsync()
        {
            _dbManager = await _dbFactory.GetDbManager(AppConstants.WasmHost.INDEXED_DB_NAME);
        }

        public async Task SetAsync<T>(string name, T data, int timeout)
            where T : IDbStoreBase
        {
            var item = await GetByNameAsync<T>(name);
            await RemoveAsync(name);
            await _dbManager.AddRecord(new StoreRecord<DbStoreSchemaBase<T>>()
            {
                StoreName = AppConstants.WasmHost.INDEXED_DB_STORE_NAME,
                Record = new DbStoreSchemaBase<T>()
                {
                    Name = name,
                    Data = data,
                    Expired = timeout > 0 ? DateTime.MinValue : DateTime.Now.AddSeconds(timeout)
                }
            });
        }

        public async Task<DbStoreSchemaBase<T>> GetAsync<T>(int id)
            where T : IDbStoreBase
        {
            return await _dbManager.GetRecordByIdAsync<int, DbStoreSchemaBase<T>>(AppConstants.WasmHost.INDEXED_DB_STORE_NAME, id);
        }

        public async Task<DbStoreSchemaBase<T>> GetByIdAsync<T>(int id)
            where T : IDbStoreBase
        {
            var result = await _dbManager.Where<DbStoreSchemaBase<T>>(AppConstants.WasmHost.INDEXED_DB_STORE_NAME, "id", id);
            return result.FirstOrDefault();
        }

        public async Task<DbStoreSchemaBase<T>> GetByNameAsync<T>(string name)
            where T : IDbStoreBase
        {
            var result = await _dbManager.Where<DbStoreSchemaBase<T>>(AppConstants.WasmHost.INDEXED_DB_STORE_NAME, "name", name);
            if (result.xIsEmpty()) return null;
            if (result.FirstOrDefault()!.Expired > DateTime.Now)
            {
                await this.RemoveAsync(name);
                return null;
            }
            return result.FirstOrDefault();
        }
        
        public async Task RemoveAsync(string key)
        {
            if (key.xIsEmpty()) return;
            var exist = await _dbManager.Where<System.Text.Json.JsonElement>(AppConstants.WasmHost.INDEXED_DB_STORE_NAME, "name", key);
            if (!exist.Any()) return;
            System.Text.Json.JsonElement result = exist.First();
            await _dbManager.DeleteRecordAsync(AppConstants.WasmHost.INDEXED_DB_STORE_NAME, result.GetProperty("id").GetInt32());
        }

        public async Task ClearAsync()
        {
            await _dbManager.ClearTableAsync(AppConstants.WasmHost.INDEXED_DB_STORE_NAME);
        }
}