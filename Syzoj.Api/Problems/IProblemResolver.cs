using System;

namespace Syzoj.Api.Problems
{
    /// <summary>
    /// The interface defines a resolver that resolves some problem of the type.
    /// </summary>
    public interface IProblemResolver
    {
        /// <summary>
        /// Returns whether the submission has been completed and its status will not change until rejudge.
        /// </summary>
        bool IsCompleted(Guid submissionId);
    }
}