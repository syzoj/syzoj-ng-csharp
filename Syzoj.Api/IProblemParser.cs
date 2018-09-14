using Syzoj.Api.Models;

namespace Syzoj.Api
{
    /// <summary>
    /// This interface defines how to parse the contents of problem folder, display
    /// it to the user, and handle submissions.
    /// </summary>
    /// <remark>
    /// The object is expected to be scoped.
    /// </remark>
    public interface IProblemParser
    {
        /// <summary>
        /// Tells whether the contents of the folder is likely to be valid
        /// for this specific problem type.
        /// </summary>
        bool IsProblemValid(Problem problem);

        /// <summary>
        /// Parses contents of the folder so the API server understands it.
        /// </summary>
        void ParseProblem(Problem problem);

        /// <summary>
        /// Returns an object describing the problem.
        /// </summary>
        object GetProblemStatement(Problem problem);

        /// <summary>
        /// Handles new submissions for the problem.
        /// </summary>
        void HandleSubmission(Problem problem, Submission submission, object data);

        /// <summary>
        /// Returns an object describing the submission.
        /// </summary>
        object GetSubmissionContent(Problem problem, Submission submission);
    }
}