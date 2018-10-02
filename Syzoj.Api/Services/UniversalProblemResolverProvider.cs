using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Problems;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Services
{
    public class UniversalProblemResolverProvider : IProblemResolverProvider
    {
        private IDictionary<string, Type> problemResolvers = new Dictionary<string, Type>()
        {
            { "standard", typeof(Syzoj.Api.Problems.Standard.StandardProblemResolverProvider) },
        };
        private readonly ApplicationDbContext dbContext;
        private readonly CacheManager cache;
        private readonly IServiceProvider provider;

        public UniversalProblemResolverProvider(ApplicationDbContext dbContext, CacheManager cache, IServiceProvider provider)
        {
            this.dbContext = dbContext;
            this.cache = cache;
            this.provider = provider;
        }
        public async Task<IProblemResolver> GetProblemResolver(Guid problemId)
        {
            var type = await cache.CachedStringAsync($"syzoj:problem:{problemId}:type", TimeSpan.FromDays(1), true, async delegate {
                var problem = await dbContext.Problems.FindAsync(new object[] { problemId });
                return problem == null ? null : problem.ProblemType;
            });
            if(type == null)
                return null;
            var resolverProvider = (IProblemResolverProvider) provider.GetRequiredService(problemResolvers[type]);
            IProblemResolver resolver = await resolverProvider.GetProblemResolver(problemId);
            if(resolver == null)
            {
                throw new NullReferenceException("GetProblemResolver returns null");
            }
            return resolver;
        }

        public async Task<ISubmissionResolver> GetSubmissionResolver(Guid problemsetId, Guid submissionId)
        {
            var type = await cache.CachedStringAsync($"syzoj:submission:{problemsetId}:{submissionId}:type", TimeSpan.FromDays(1), true, async delegate {
                return await dbContext.Submissions.Where(s => s.ProblemsetId == problemsetId && s.Id == submissionId).Select(s => s.Problem.ProblemType).FirstOrDefaultAsync();
            });
            if(type == null)
                return null;
            var resolverProvider = (IProblemResolverProvider) provider.GetRequiredService(problemResolvers[type]);
            ISubmissionResolver resolver = await resolverProvider.GetSubmissionResolver(problemsetId, submissionId);
            if(resolver == null)
            {
                throw new NullReferenceException("GetSubmissionResolver returns null");
            }
            return resolver;
        }
    }
}