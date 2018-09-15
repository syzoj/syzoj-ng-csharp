using System.Threading.Tasks;
using Syzoj.Api.Models;

namespace Syzoj.Api
{
    public class DebugProblemsetManager : IAsyncProblemsetManager
    {
        public Task<bool> IsProblemEditableAsync(Problemset problemset, ProblemsetProblem problem)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemListVisibleAsync(Problemset problemset)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemsetEditableAsync(Problemset problemset)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemSubmittableAsync(Problemset problemset, ProblemsetProblem problem)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemViewableAsync(Problemset problemset, ProblemsetProblem problem)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsProblemVisibleAsync(Problemset problemset, ProblemsetProblem problem)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsSubmissionInteractableAsync(Problemset problemset, Submission submission)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsSubmissionViewableAsync(Problemset problemset, Submission submission)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsSubmissionListVisibleAsync(Problemset problemset)
        {
            return Task.FromResult(true);
        }
    }
}