using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemResolverService
    {
        Task<IProblemResolver> GetProblemResolver(Guid problemId);
        Task<IProblemResolver> CreateNewProblem(string problemType);
    }
}