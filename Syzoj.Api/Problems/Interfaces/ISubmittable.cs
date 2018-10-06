using System;
using System.Threading.Tasks;
using Syzoj.Api.Events;

namespace Syzoj.Api.Problems.Interfaces
{
    /// <summary>
    /// Defines the contract for problems that implement their own submission logic.
    /// The problem checks submission permissions by emitting <see cref="ICancellableEvent" />.
    /// </summary>
    public interface ISubmittable : IProblemResolver
    {
        /// <summary>
        /// Creates an empty submission with the given submissionId.
        /// The problem handles the submission in its own namespace.
        /// </summary>
        Task CreateSubmissionAsync(Guid submissionId);
    }
}