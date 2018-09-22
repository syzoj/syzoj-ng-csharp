using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Services
{
    public class DefaultProblemsetManager : IAsyncProblemsetManager, IAsyncProblemsetPermissionManager
    {
        public Task AttachProblem(Guid problemsetId, Guid problemId, string name)
        {
            throw new NotImplementedException();
        }

        public Task ChangeProblemName(Guid problemsetId, Guid problemId, string newName)
        {
            throw new NotImplementedException();
        }

        public Task DetachProblem(Guid problemsetId, Guid problemId)
        {
            throw new NotImplementedException();
        }

        public Task DoGarbageCollect()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProblemEditableAsync(Guid problemsetId, Guid problemId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProblemListVisibleAsync(Guid problemsetId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProblemsetEditableAsync(Guid problemsetId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProblemSubmittableAsync(Guid problemsetId, Guid problemId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProblemViewableAsync(Guid problemsetId, Guid problemId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSubmissionInteractableAsync(Guid problemsetId, Guid submissionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSubmissionListVisibleAsync(Guid problemsetId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSubmissionViewableAsync(Guid problemsetId, Guid submissionId)
        {
            throw new NotImplementedException();
        }

        public Task NewSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission)
        {
            throw new NotImplementedException();
        }

        public Task PatchProblem(Guid problemsetId, Guid problemId, object problem)
        {
            throw new NotImplementedException();
        }

        public Task PatchSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission)
        {
            throw new NotImplementedException();
        }

        public Task PutProblem(Guid problemsetId, Guid problemId, object problem)
        {
            throw new NotImplementedException();
        }

        public Task PutSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission)
        {
            throw new NotImplementedException();
        }

        public Task RebuildIndex()
        {
            throw new NotImplementedException();
        }
    }
}