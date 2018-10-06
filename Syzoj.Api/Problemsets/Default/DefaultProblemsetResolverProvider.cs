using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems;
using Syzoj.Api.Problems.Interfaces;

namespace Syzoj.Api.Problemsets.Default
{
    public class DefaultProblemsetResolverProvider : IProblemsetResolverProvider
    {
        public string ProblemsetType => "default";

        public Task<IProblemsetResolver> GetProblemsetResolver(IServiceProvider serviceProvider, Guid problemsetId)
        {
            return Task.FromResult<IProblemsetResolver>(new DefaultProblemsetResolver(serviceProvider, problemsetId));
        }
    }
}