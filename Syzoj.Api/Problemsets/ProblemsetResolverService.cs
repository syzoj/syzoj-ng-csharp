using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using Syzoj.Api.Models;

namespace Syzoj.Api.Problemsets
{
    public class ProblemsetResolverService : IProblemsetResolverService
    {
        private readonly ProblemsetResolverDictionary dict;
        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;

        public ProblemsetResolverService(ProblemsetResolverDictionary dict, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            this.dict = dict;
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public async Task<IProblemsetResolver> GetProblemsetResolver(Guid problemsetId)
        {
            var problemset = await context.Set<Problemset>().FindAsync(problemsetId);
            if(problemset == null)
                return null;
            
            IProblemsetResolverProvider provider = dict.GetProvider(problemset.Type);
            return await provider.GetProblemsetResolver(serviceProvider, problemsetId);
        }
    }
}