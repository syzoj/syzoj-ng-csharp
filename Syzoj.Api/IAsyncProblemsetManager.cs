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
        Task<bool> IsProblemsetEditableAsync(Problemset problemset);
        /// <summary>
        /// Returns whether the problem list should be visible to the
        /// current user.
        /// </summary>
        Task<bool> IsProblemListVisibleAsync(Problemset problemset);

        /// <summary>
        /// Returns whether the problem is viewable to the current user.
        /// </summary>
        Task<bool> IsProblemViewableAsync(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can submit to the problem.
        /// </summary>
        Task<bool> IsProblemSubmittableAsync(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can edit the problem data.
        /// </summary>
        Task<bool> IsProblemEditableAsync(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can see the list of submissions.
        /// </summary>
        Task<bool> IsSubmissionListVisibleAsync(Problemset problemset);

        /// <summary>
        /// Returns whether the current user can view the specified submission.
        /// </summary>
        Task<bool> IsSubmissionViewableAsync(Problemset problemset, Submission submission);

        /// <summary>
        /// Returns whether the current user can interact with the specified submission.
        /// </summary>
        Task<bool> IsSubmissionInteractableAsync(Problemset problemset, Submission submission);
    }
}