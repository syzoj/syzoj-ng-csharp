using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemResolverService
    {
        Task<IProblemResolver> GetProblemResolver(Guid problemId);
        Task RegisterProblem(Guid problemId, string problemType);
    }
}