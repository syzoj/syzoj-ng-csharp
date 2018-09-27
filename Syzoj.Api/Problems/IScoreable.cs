using System;

namespace Syzoj.Api.Problems
{
    /// <summary>
    /// The interface defines a problem type that has scores with every submission.
    /// </summary>
    public interface IScoreable : IProblemResolver
    {
        /// <summary>
        /// Gets the score of a specific submission if the submission has a score.
        /// </summary>
        decimal? GetScore(Guid submissionId);
    }
}