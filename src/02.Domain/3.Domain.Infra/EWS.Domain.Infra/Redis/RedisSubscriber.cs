using Serilog;
using Serilog.Core;
using StackExchange.Redis;

namespace EWS.Domain.Infra.Redis;

public class RedisSubscriber
{
    private readonly ISubscriber _subscriber;
    public RedisSubscriber(ISubscriber subscriber)
    {
        _subscriber = subscriber;
    }

    public async Task SubscribeToChannelAsync(string messageChannel, Func<RedisChannel, RedisValue, Task> func)
    {
        async void Handler(RedisChannel redisChannel, RedisValue value)
        {
            Log.Logger.Debug("SubscribeToChannelAsync - Channel:{Channel}, Value:{Value}", redisChannel, value);
            await func(redisChannel, value);
        }
        await _subscriber.SubscribeAsync(messageChannel, Handler);
    }
}