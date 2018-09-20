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
        /// Tells whether the contents of the folder is likely to be valid
        /// for this specific problem type.
        /// </summary>
        Task<bool> IsProblemValidAsync(Guid problemId);

        /// <summary>
        /// Parses contents of the folder so the API server understands it.
        /// </summary>
        Task ParseProblemAsync(Guid problemId);

        /// <summary>
        /// Handles new submissions for the problem.
        /// </summary>
        Task HandleSubmissionAsync(Guid problemId, Guid submissionId, object data);
    }
}