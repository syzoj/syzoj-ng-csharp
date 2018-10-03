using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Models;

namespace Syzoj.Api.Problems
{
    public abstract class ProblemResolverBase : IProblemResolver
    {
        public Guid Id { get; }
        protected IServiceProvider ServiceProvider { get; }
        public ProblemResolverBase(IServiceProvider serviceProvider, Guid problemId)
        {
            this.Id = problemId;
            this.ServiceProvider = serviceProvider;
        }

        protected virtual Task<Problem> GetProblemModel()
        {
            return ServiceProvider.GetRequiredService<ApplicationDbContext>()
                .Problems.FindAsync(Id);
        }
    }
}