using System;

namespace Syzoj.Api.Problems
{
    /// <summary>
    /// The interface defines a resolver that resolves some submission of the type.
    /// </summary>
    public interface ISubmissionResolver
    {
        /// <summary>
        /// The ID of the submission.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The ID of the problemset.
        /// </summary>
        Guid problemsetId { get; }
    }
}