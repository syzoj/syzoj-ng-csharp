using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using MessagePack;
using StackExchange.Redis;

namespace Syzoj.Api.Services
{
    public class CacheManager
    {
        private readonly IConnectionMultiplexer redis;

        public CacheManager(IConnectionMultiplexer redis)
        {
            this.redis = redis;
        }

        public async Task<T> CachedAsync<T>(string keyName, TimeSpan expiry, bool renewOnRead, Func<Task<T>> func)
        {
            var val = await redis.GetDatabase().StringGetAsync(keyName);
            if(val.IsNullOrEmpty)
            {
                var result = await func.Invoke();
                var stream = new MemoryStream();
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, result);
                redis.GetDatabase().StringSet(keyName, stream.ToArray(), expiry, flags: CommandFlags.FireAndForget);
                return result;
            }
            else
            {
                if(renewOnRead)
                {
                    redis.GetDatabase().KeyExpire(keyName, expiry, CommandFlags.FireAndForget);
                }
                var stream = new MemoryStream((byte[]) val);
                IFormatter formatter = new BinaryFormatter();
                var result = formatter.Deserialize(stream);
                return (T) result;
            }
        }

        public async Task<T> CachedMessagePackAsync<T>(string keyName, TimeSpan expiry, bool renewOnRead, Func<Task<T>> func)
        {
            var val = await redis.GetDatabase().StringGetAsync(keyName);
            if(val.IsNullOrEmpty)
            {
                var result = await func.Invoke();
                var data = MessagePackSerializer.Serialize(result);
                redis.GetDatabase().StringSet(keyName, data, expiry, flags: CommandFlags.FireAndForget);
                return result;
            }
            else
            {
                if(renewOnRead)
                {
                    redis.GetDatabase().KeyExpire(keyName, expiry, CommandFlags.FireAndForget);
                }
                return MessagePackSerializer.Deserialize<T>(val);
            }
        }

        public async Task<string> CachedStringAsync(string keyName, TimeSpan expiry, bool renewOnRead, Func<Task<string>> func)
        {
            var val = await redis.GetDatabase().StringGetAsync(keyName);
            if(val.IsNullOrEmpty)
            {
                var result = await func.Invoke();
                redis.GetDatabase().StringSet(keyName, result, expiry, flags: CommandFlags.FireAndForget);
                return result;
            }
            else
            {
                if(renewOnRead)
                {
                    redis.GetDatabase().KeyExpire(keyName, expiry, CommandFlags.FireAndForget);
                }
                return val;
            }
        }
    }
}