using System;
using System.Threading.Tasks;
using Syzoj.Api.Events;
using Syzoj.Api.Problems;
using Syzoj.Api.Problems.Interfaces;

namespace Syzoj.Api.Problemsets.Default
{
    public class DefaultProblemsetResolver : ProblemsetResolverBase
    {
        public DefaultProblemsetResolver(IServiceProvider serviceProvider, Guid problemsetId) : base(serviceProvider, problemsetId)
        {
        }

        public Task<bool> IsProblemAcceptable(IProblemResolver problem)
        {
            return Task.FromResult(problem is ISubmittable && problem is IViewable);
        }
    }
}