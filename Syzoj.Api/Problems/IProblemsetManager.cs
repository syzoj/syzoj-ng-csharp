using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Problems.Permission;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetManager
    {
        /// <summary>
        /// The ID of the problemset.
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// Returns the permission manager for current user.
        /// Note that userId equals Guid.Empty when user is not logged in.
        /// </summary>
        Task<IPermissionManager<ProblemsetPermission>> GetProblemsetPermissionManagerAsync(Guid userId);

        /// <summary>
        /// Returns the permission manager for a certain problem for current user.
        /// Note that userId equals Guid.Empty when user is not logged in.
        /// </summary>
        Task<IPermissionManager<ProblemPermission>> GetProblemPermissionManagerAsync(Guid userId, Guid problemId);

        /// <summary>
        /// Returns the permission manager for a certain submission for current user.
        /// Note that userId equals Guid.Empty when user is not logged in.
        /// </summary>
        Task<IPermissionManager<SubmissionPermission>> GetSubmissionPermissionManagerAsync(Guid userId, Guid submissionId);
    }
}