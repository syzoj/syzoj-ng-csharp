using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemResolverProvider
    {
        Task<IProblemResolver> GetProblemResolver(Guid problemId);
        Task<ISubmissionResolver> GetSubmissionResolver(Guid problemsetId, Guid submissionId);
    }
}