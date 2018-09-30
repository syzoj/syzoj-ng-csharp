using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems;
using Syzoj.Api.Problems.Permission;

namespace Syzoj.Api.Services
{
    public class DebugProblemsetManagerProvider : IProblemsetManagerProvider
    {
        public Task<IProblemsetManager> GetProblemsetManager(Guid problemsetId)
        {
            return Task.FromResult<IProblemsetManager>(new DebugProblemsetManager(problemsetId));
        }

        private class DebugProblemsetManager : IProblemsetManager
        {
            public Guid Id { get; }
            public DebugProblemsetManager(Guid Id)
            {
                this.Id = Id;
            }
            public Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid userId, Guid problemId)
            {
                return Task.FromResult<IPermissionManager<ProblemPermission>>(new DebugPermissionManager<ProblemPermission>());
            }

            public Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync(Guid userId)
            {
                return Task.FromResult<IPermissionManager<ProblemsetPermission>>(new DebugPermissionManager<ProblemsetPermission>());
            }

            public Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid userId, Guid submissionId)
            {
                return Task.FromResult<IPermissionManager<SubmissionPermission>>(new DebugPermissionManager<SubmissionPermission>());
            }
        }
    }
}