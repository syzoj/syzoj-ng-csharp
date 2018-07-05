using Microsoft.Extensions.Options;

namespace Syzoj.Api.Utils.RedisUtils
{
    public class ApplicationRedisClientOptions : IOptions<ApplicationRedisClientOptions>
    {
        public string ConfigurationString { get; set; }
        public ApplicationRedisClientOptions Value => this;
    }
}