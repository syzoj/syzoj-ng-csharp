using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetPermissionManager
    {
        /// <summary>
        /// Returns the permission manager for a certain problemset for current user.
        /// </summary>
        Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync(Guid problemsetId);

        /// <summary>
        /// Returns the permission manager for a certain problem in a problemset for current user.
        /// </summary>
        Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid problemsetId, Guid problemId);

        /// <summary>
        /// Returns the permission manager for a certain submission in a problemset for current user.
        /// </summary>
        Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid problemsetId, Guid submissionId);
    }
}