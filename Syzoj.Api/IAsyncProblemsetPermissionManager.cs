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
        /// Common permission names include:
        /// - view: View the list of problems.
        /// - edit: Change problemset, such as changing problem names.
        /// - create: Create new problems. `edit` permission is usually also required.
        /// </summary>
        Task<bool> CheckProblemsetPermissionAsync(Guid problemsetId, string name);

        /// <summary>
        /// Returns whether the user has certain permission to a problem.
        /// Common permission names include:
        /// - view: View the problem statement.
        /// - edit: Edit the problem.
        /// - submit: Submit to the problem.
        /// - export: Export whole problem, including related files.
        /// </summary>
        /// <remark>
        /// Editing the problem also requires the 'export' privilege.
        /// </remark>
        Task<bool> CheckProblemPermissionAsync(Guid problemsetId, Guid problemId, string name);

        /// <summary>
        /// Returns whether the user has certain permission to a submission.
        /// </summary>
        Task<bool> CheckSubmissionPermissionAsync(Guid problemsetId, Guid submissionId, string name);
    }
}