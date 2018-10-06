using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Problemsets;

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

        public abstract Task<bool> IsProblemsetAcceptable(IProblemsetResolver problemsetResolver);
    }
}