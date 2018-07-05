using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Syzoj.Api.Utils.RedisUtils
{
    // We are not using Microsoft.Extensions.Caching.Redis because
    // we want advanced control over the data.
    
    // TODO: Refine the following code
    public class ApplicationRedisClient : IDisposable
    {
        // The following content had been partially copied from
        // https://github.com/aspnet/Caching/blob/f6ffa1a4d3fed10acea0ea773c837be3ecc81d43/src/Microsoft.Extensions.Caching.Redis/RedisCache.cs
        // with modifications.
        
        private volatile ConnectionMultiplexer _connection;
        private IDatabase _cache;

        private readonly ConfigurationOptions _options;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public IDatabase RedisDatabase
        {
            get
            {
                // The Connect() operation should be async; but let's use sync for simplicity for now.
                Connect();
                return _connection.GetDatabase();
            }
        }

        public ApplicationRedisClient(IOptions<ApplicationRedisClientOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = ConfigurationOptions.Parse(optionsAccessor.Value.ConfigurationString);
        }

        private void Connect()
        {
            if (_cache != null)
            {
                return;
            }

            _connectionLock.Wait();
            try
            {
                if (_cache == null)
                {
                    _connection = ConnectionMultiplexer.Connect(_options);                        
                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private async Task ConnectAsync(CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            if (_cache != null)
            {
                return;
            }

            await _connectionLock.WaitAsync(token);
            try
            {
                if (_cache == null)
                {
                    _connection = await ConnectionMultiplexer.ConnectAsync(_options);                  
                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
        
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        } 
    }
}