using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Services
{
    public class DefaultProblemsetPermissionManager : IAsyncProblemsetPermissionManager
    {
        public Task<bool> CheckProblemPermissionAsync(Guid problemsetId, Guid problemId, string name)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CheckProblemsetPermissionAsync(Guid problemsetId, string name)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CheckSubmissionPermissionAsync(Guid problemsetId, Guid submissionId, string name)
        {
            return Task.FromResult(true);
        }
    }
}