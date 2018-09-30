using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Problems;

namespace Syzoj.Api.Services
{
    public class UniversalProblemsetManagerProvider : IProblemsetManagerProvider
    {
        private IDictionary<string, Type> problemsetManagers = new Dictionary<string, Type>()
        {
            { "default", typeof(DebugProblemsetManagerProvider) },
        };
        private readonly ApplicationDbContext dbContext;
        private readonly CacheManager cache;
        private readonly IServiceProvider provider;

        public UniversalProblemsetManagerProvider(ApplicationDbContext dbContext, CacheManager cache, IServiceProvider provider)
        {
            this.dbContext = dbContext;
            this.cache = cache;
            this.provider = provider;
        }
        public async Task<IProblemsetManager> GetProblemsetManager(Guid problemsetId)
        {
            var type = await cache.CachedStringAsync($"syzoj:problemset:{problemsetId}:type", TimeSpan.FromDays(1), true, async delegate {
                var problemset = await dbContext.Problemsets.FindAsync(new object[] { problemsetId });
                return problemset == null ? null : problemset.Type;
            });
            if(type == null)
                return null;
            var managerProvider = (IProblemsetManagerProvider) provider.GetRequiredService(problemsetManagers[type]);
            IProblemsetManager manager = await managerProvider.GetProblemsetManager(problemsetId);
            if(manager == null)
            {
                throw new NullReferenceException("GetProblemsetManager returns null");
            }
            return manager;
        }
    }
}