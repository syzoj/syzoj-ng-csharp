using System;
using StackExchange.Redis;
using Syzoj.Api.Utils.RedisUtils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MyRedisClientServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisClient(this IServiceCollection services, Action<ApplicationRedisClientOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<ApplicationRedisClient, ApplicationRedisClient>());

            return services;
        }
    }
}