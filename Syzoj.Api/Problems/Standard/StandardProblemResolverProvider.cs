using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemResolverProvider : IProblemResolverProvider
    {
        public string ProblemType => "standard";

        public Task<IProblemResolver> GetProblemResolver(IServiceProvider serviceProvider, Guid problemId)
        {
            return Task.FromResult<IProblemResolver>(new StandardProblemResolver(serviceProvider, problemId));
        }
    }
}