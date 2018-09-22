using System;
using System.Threading.Tasks;
using Syzoj.Api.Models;

namespace Syzoj.Api
{
    /// <summary>
    /// This interface defines whether a given user is authorized to do
    /// specified action under certain circumstances.
    /// </summary>
    public interface IAsyncProblemsetPermissionManager
    {
        /// <summary>
        /// Returns whether the user has certain permission to a problemset.
        /// </summary>
        Task<bool> CheckProblemsetPermissionAsync(Guid problemsetId, string name);

        /// <summary>
        /// Returns whether the user has certain permission to a problem.
        /// </summary>
        Task<bool> CheckProblemPermissionAsync(Guid problemsetId, Guid problemId, string name);

        /// <summary>
        /// Returns whether the user has certain permission to a submission.
        /// </summary>
        Task<bool> CheckSubmissionPermissionAsync(Guid problemsetId, Guid submissionId, string name);
    }
}