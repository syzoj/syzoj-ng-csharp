using System;
using System.Threading.Tasks;
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
    public interface IAsyncProblemParser
    {
        /// <summary>
        /// Gets the summary of the problem.
        /// </summary>
        Task<object> GetProblemSummaryAsync(Guid problemId);

        /// <summary>
        /// Gets the description of the problem.
        /// </summary>
        Task<object> GetProblemDescriptionAsync(Guid problemId);

        /// <summary>
        /// Handles new submissions for the problem.
        /// </summary>
        Task HandleSubmissionAsync(Guid problemId, Guid submissionId);
    }
}