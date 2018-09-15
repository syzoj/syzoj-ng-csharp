using System.Threading.Tasks;
using Syzoj.Api.Models;

namespace Syzoj.Api
{
    /// <summary>
    /// This interface defines whether a given user is authorized to do
    /// specified action under certain circumstances.
    /// </summary>
    public interface IProblemsetPermissionManager
    {
        /// <summary>
        /// Returns whether the problem list should be visible to the
        /// current user.
        /// </summary>
        Task<bool> IsProblemListVisible(Problemset problemset);

        /// <summary>
        /// Returns whether the problem is visible in the problem list to
        /// the current user.
        /// </summary>
        /// <remark>
        /// It is not recommended to hide problems from users in this way
        /// because the problem behaves as if it was removed after the list
        /// is generated, so a page intending to show 10 problems may end up
        /// showing nothing. You should always return true.
        /// </remark>
        Task<bool> IsProblemVisible(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the problem is viewable to the current user.
        /// </summary>
        Task<bool> IsProblemViewable(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can submit to the problem.
        /// </summary>
        Task<bool> IsProblemSubmittable(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can edit the problem data.
        /// </summary>
        Task<bool> IsProblemEditable(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the submission is visible in the submission history to
        /// the current user.
        /// </summary>
        Task<bool> IsSubmissionVisible(Problemset problemset, Submission submission);

        /// <summary>
        /// Returns whether the current user can view the specified submission.
        /// </summary>
        Task<bool> IsSubmissionViewable(Problemset problemset, Submission submission);

        /// <summary>
        /// Returns whether the current user can interact with the specified submission.
        /// </summary>
        Task<bool> IsSubmissionInteractable(Problemset problemset, Submission submission);
    }
}