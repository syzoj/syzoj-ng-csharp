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
        bool IsProblemListVisible(Problemset problemset);

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
        bool IsProblemVisible(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the problem is viewable to the current user.
        /// </summary>
        bool IsProblemViewable(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can submit to the problem.
        /// </summary>
        bool IsProblemSubmittable(Problemset problemset, ProblemsetProblem problem);

        /// <summary>
        /// Returns whether the current user can view the specified submission.
        /// </summary>
        bool IsSubmissionViewable(Problemset problemset, Submission submission);

        /// <summary>
        /// Returns whether the current user can interact with the specified submission.
        /// </summary>
        bool IsSubmissionInteractable(Problemset problemset, Submission submission);
    }
}