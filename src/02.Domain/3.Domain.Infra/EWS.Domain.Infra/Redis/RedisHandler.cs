using EWS.Application.Const;
using EWS.Domain.Example;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace EWS.Domain.Infra.Redis;

public class RedisHandler
{
    private readonly IConnectionMultiplexer _redis;
    public RedisHandler(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    /// <summary>
    /// 작업자 저장 후 Publish
    /// </summary>
    /// <param name="channel">내가 속한 채널명</param>
    /// <param name="key">데이터를 조회할 키</param>
    /// <param name="redisWorkerProtocol">작업자 프로토콜</param>
    public async Task SetAndPublishAsync(string key, RedisWorkerProtocol redisWorkerProtocol)
    {
        var db = _redis.GetDatabase(0);
        await db.StringSetAsync(key, redisWorkerProtocol.xToJson());
        var subscriber = _redis.GetSubscriber();
        await subscriber.PublishAsync(ApplicationConstants.Redis.MessageChannel, key);
    }

    public async Task<string> GetAsync(string key)
    {
        var db = _redis.GetDatabase(0);
        var result = await db.StringGetAsync(key);
        return (string)result;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var db = _redis.GetDatabase(0);
        var result = await db.StringGetAsync(key);
        return ((string)result).xToEntity<T>();
    }

    public async Task SetAsync(string key, string value)
    {
        var db = _redis.GetDatabase(0);
        await db.StringSetAsync(key, value);
    }

    public async Task SetAsync<T>(string key, T item)
    {
        var db = _redis.GetDatabase(0);
        await db.StringSetAsync(key, item.xToJson());
    }

    public async Task RemoveAsync(string key)
    {
        var db = _redis.GetDatabase(0);
        await db.KeyDeleteAsync(key);
    }
}