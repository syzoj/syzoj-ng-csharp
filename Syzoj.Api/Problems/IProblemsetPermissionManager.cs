using System;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetPermissionManager
    {
        /// <summary>
        /// Returns the permission manager for a certain problemset for current user.
        /// </summary>
        IPermissionManager<ProblemsetPermission> GetProblemsetPermissionManager(Guid problemsetId);

        /// <summary>
        /// Returns the permission manager for a certain problem in a problemset for current user.
        /// </summary>
        IPermissionManager<ProblemPermission> GetProblemPermissionManager(Guid problemsetId, Guid problemId);

        /// <summary>
        /// Returns the permission manager for a certain submission in a problemset for current user.
        /// </summary>
        IPermissionManager<SubmissionPermission> GetSubmissionPermissionManager(Guid problemsetId, Guid submissionId);
    }
}