using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems;
using Syzoj.Api.Problems.Permission;

namespace Syzoj.Api.Services
{
    public class DebugProblemsetManagerFactory : IProblemsetManagerFactory
    {
        public Task<IProblemsetManager> GetProblemsetManager(Guid problemsetId)
        {
            return Task.FromResult<IProblemsetManager>(new DebugProblemsetManager());
        }

        private class DebugProblemsetManager : IProblemsetManager
        {
            public Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid problemId)
            {
                return Task.FromResult<IPermissionManager<ProblemPermission>>(new DebugPermissionManager<ProblemPermission>());
            }

            public Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync()
            {
                return Task.FromResult<IPermissionManager<ProblemsetPermission>>(new DebugPermissionManager<ProblemsetPermission>());
            }

            public Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid submissionId)
            {
                return Task.FromResult<IPermissionManager<SubmissionPermission>>(new DebugPermissionManager<SubmissionPermission>());
            }
        }
    }
}