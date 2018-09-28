using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Problems;

namespace Syzoj.Api.Services
{
    public class UniversalProblemsetPermissionManager : IProblemsetPermissionManager
    {
        private static IDictionary<string, Type> permissionManagers = new Dictionary<string, Type>()
        {
            { "debug", typeof(DebugProblemsetPermissionManager) },
        };
        private readonly CacheManager cache;
        private readonly ApplicationDbContext dbContext;
        private readonly IServiceProvider provider;

        public UniversalProblemsetPermissionManager(CacheManager cache, ApplicationDbContext dbContext, IServiceProvider provider)
        {
            this.cache = cache;
            this.dbContext = dbContext;
            this.provider = provider;
        }

        public async Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid problemsetId, Guid problemId)
        {
            var problemsetType = await cache.CachedStringAsync($"syzoj:problemset:{problemsetId}:type", TimeSpan.FromDays(7), true, async delegate {
                var problemset = await dbContext.Problemsets.FindAsync(new object[] { problemsetId });
                if(problemset != null)
                    return problemset.Type;
                return null;
            });
            if(problemsetType == null)
                return null;
            var manager = (IProblemsetPermissionManager) provider.GetRequiredService(permissionManagers[problemsetType]);
            return await manager.GetProblemPermissionManagerAsync(problemsetId, problemId);
        }

        public async Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync(Guid problemsetId)
        {
            var problemsetType = await cache.CachedStringAsync($"syzoj:problemset:{problemsetId}:type", TimeSpan.FromDays(7), true, async delegate {
                var problemset = await dbContext.Problemsets.FindAsync(new object[] { problemsetId });
                if(problemset != null)
                    return problemset.Type;
                return null;
            });
            if(problemsetType == null)
                return null;
            var manager = (IProblemsetPermissionManager) provider.GetRequiredService(permissionManagers[problemsetType]);
            return await manager.GetProblemsetPermissionManagerAsync(problemsetId);
        }

        public async Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid problemsetId, Guid submissionId)
        {
            var problemsetType = await cache.CachedStringAsync($"syzoj:problemset:{problemsetId}:type", TimeSpan.FromDays(7), true, async delegate {
                var problemset = await dbContext.Problemsets.FindAsync(new object[] { problemsetId });
                if(problemset != null)
                    return problemset.Type;
                return null;
            });
            if(problemsetType == null)
                return null;
            var manager = (IProblemsetPermissionManager) provider.GetRequiredService(permissionManagers[problemsetType]);
            return await manager.GetSubmissionPermissionManagerAsync(problemsetId, submissionId);
        }
    }
}