using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface ISubmittable : IProblemResolver
    {
        Task CreateSubmissionAsync(Guid submissionId);
    }
}