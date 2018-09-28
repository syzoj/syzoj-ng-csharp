using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Problems;

namespace Syzoj.Api.Services
{
    public class UniversalProblemsetManagerFactory : IProblemsetManagerFactory
    {
        private IDictionary<string, Type> problemsetManagers = new Dictionary<string, Type>()
        {
            { "debug", typeof(DebugProblemsetManagerFactory) },
        };
        private readonly ApplicationDbContext dbContext;
        private readonly CacheManager cache;
        private readonly IServiceProvider provider;

        public UniversalProblemsetManagerFactory(ApplicationDbContext dbContext, CacheManager cache, IServiceProvider provider)
        {
            this.dbContext = dbContext;
            this.cache = cache;
            this.provider = provider;
        }
        public async Task<IProblemsetManager> GetProblemsetManager(Guid problemsetId)
        {
            var type = await cache.CachedStringAsync($"syzoj:problemset:{problemsetId}:type", TimeSpan.FromDays(1), true, async delegate {
                var problemset = await dbContext.Problemsets.FindAsync(new object[] { problemsetId });
                return problemset.Type;
            });
            return (IProblemsetManager) provider.GetRequiredService(problemsetManagers[type]);
        }
    }
}