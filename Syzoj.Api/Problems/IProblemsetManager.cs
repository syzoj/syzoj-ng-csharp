using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems.Permission;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetManager
    {
        /// <summary>
        /// Returns the permission manager for current user.
        /// </summary>
        Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync();

        /// <summary>
        /// Returns the permission manager for a certain problem for current user.
        /// </summary>
        Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid problemId);

        /// <summary>
        /// Returns the permission manager for a certain submission for current user.
        /// </summary>
        Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid submissionId);
    }
}