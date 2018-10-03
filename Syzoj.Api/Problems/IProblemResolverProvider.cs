using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemResolverProvider
    {
        string ProblemType { get; }
        Task<IProblemResolver> GetProblemResolver(IServiceProvider serviceProvider, Guid problemId);
        Task<IProblemResolver> CreateProblem(IServiceProvider serviceProvider, Guid problemId);
    }
}