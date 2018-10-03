using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Models;

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

        public async Task<IProblemResolver> CreateNewProblem(string problemType)
        {
            var problemId = Guid.NewGuid();
            var problem = new Problem()
            {
                Id = problemId,
                ProblemType = problemType,
                Content = "null",
                Data = null,
            };
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Problems.Add(problem);

            IProblemResolverProvider provider = dict.GetProvider(problem.ProblemType);
            var problemResolver = await provider.CreateProblem(serviceProvider, problemId);
            await context.SaveChangesAsync();
            return problemResolver;
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