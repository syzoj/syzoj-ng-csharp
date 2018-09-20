using System;
using System.Threading.Tasks;
using Syzoj.Api.Models;

namespace Syzoj.Api
{
    public class DebugProblemsetManager : IAsyncProblemsetManager
    {
        public Task<bool> IsProblemEditableAsync(Guid problemsetId, Guid problemId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemListVisibleAsync(Guid problemsetId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemsetEditableAsync(Guid problemsetId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemSubmittableAsync(Guid problemsetId, Guid problemId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemViewableAsync(Guid problemsetId, Guid problemId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsSubmissionInteractableAsync(Guid problemsetId, Guid submissionId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsSubmissionListVisibleAsync(Guid problemsetId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsSubmissionViewableAsync(Guid problemsetId, Guid submissionId)
        {
            return Task.FromResult(true);
        }
    }
}