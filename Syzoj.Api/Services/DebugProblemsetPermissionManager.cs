using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems;

namespace Syzoj.Api.Services
{
    public class DebugProblemsetPermissionManager : IProblemsetPermissionManager
    {
        public Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid problemsetId, Guid problemId)
        {
            return Task.FromResult<IPermissionManager<ProblemPermission>>(new DebugPermissionManager<ProblemPermission>());
        }

        public Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync(Guid problemsetId)
        {
            return Task.FromResult<IPermissionManager<ProblemsetPermission>>(new DebugPermissionManager<ProblemsetPermission>());
        }

        public Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid problemsetId, Guid submissionId)
        {
            return Task.FromResult<IPermissionManager<SubmissionPermission>>(new DebugPermissionManager<SubmissionPermission>());
        }
    }
}