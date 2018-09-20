using System;
using System.Threading.Tasks;
using Syzoj.Api.Models;

namespace Syzoj.Api
{
    /// <summary>
    /// This interface defines whether a given user is authorized to do
    /// specified action under certain circumstances.
    /// </summary>
    public interface IAsyncProblemsetManager
    {
        /// <summary>
        /// Returns whether the user can edit the problemset.
        /// </summary>
        Task<bool> IsProblemsetEditableAsync(Guid problemsetId);
        /// <summary>
        /// Returns whether the problem list should be visible to the
        /// current user.
        /// </summary>
        Task<bool> IsProblemListVisibleAsync(Guid problemsetId);

        /// <summary>
        /// Returns whether the problem is viewable to the current user.
        /// </summary>
        Task<bool> IsProblemViewableAsync(Guid problemsetId, Guid problemId);

        /// <summary>
        /// Returns whether the current user can submit to the problem.
        /// </summary>
        Task<bool> IsProblemSubmittableAsync(Guid problemsetId, Guid problemId);

        /// <summary>
        /// Returns whether the current user can edit the problem data.
        /// </summary>
        Task<bool> IsProblemEditableAsync(Guid problemsetId, Guid problemId);

        /// <summary>
        /// Returns whether the current user can see the list of submissions.
        /// </summary>
        Task<bool> IsSubmissionListVisibleAsync(Guid problemsetId);

        /// <summary>
        /// Returns whether the current user can view the specified submission.
        /// </summary>
        Task<bool> IsSubmissionViewableAsync(Guid problemsetId, Guid submissionId);

        /// <summary>
        /// Returns whether the current user can interact with the specified submission.
        /// </summary>
        Task<bool> IsSubmissionInteractableAsync(Guid problemsetId, Guid submissionId);
    }
}