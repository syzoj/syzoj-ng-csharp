using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problems
{
    public class ProblemResolverService : IProblemResolverService
    {
        private readonly ProblemResolverDictionary dict;
        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;

        public ProblemResolverService(ProblemResolverDictionary dict, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            this.dict = dict;
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public async Task<IProblemResolver> GetProblemResolver(Guid problemId)
        {
            var problem = await context.Problems.FindAsync(problemId);
            if(problem == null)
                return null;
            
            IProblemResolverProvider provider = dict.GetProvider(problem.ProblemType);
            return await provider.GetProblemResolver(serviceProvider, problemId);
        }
    }
}