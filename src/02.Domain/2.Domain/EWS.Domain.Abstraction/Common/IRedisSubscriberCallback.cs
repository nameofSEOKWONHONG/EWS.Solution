using System.Threading.Tasks;
using StackExchange.Redis;

namespace EWS.Domain.Abstraction.Common;

public interface IRedisSubscriberCallback
{
    Task Callback(RedisChannel channel, RedisValue value);
}