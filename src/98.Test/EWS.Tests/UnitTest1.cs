using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace EWS.Tests;

public class Tests
{
    private ConnectionMultiplexer _redis;
    private IDatabase _db;
    private IJsonCommands _jsonCommands;
    private ISearchCommands _searchCommands;
    private ISubscriber _sub;
    [SetUp]
    public void Setup()
    {
        _redis = ConnectionMultiplexer.Connect("172.27.10.174");
        _db = _redis.GetDatabase();
        _sub   = _redis.GetSubscriber();
    }

    [Test]
    public async Task Test1()
    {
        var job = "hello";
        await _sub.PublishAsync("messages", job);
    }
}