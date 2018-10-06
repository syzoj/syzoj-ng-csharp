using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problemsets.Interfaces
{
    /// <summary>
    /// Defines the contract that checks permissions for submissions.
    /// </summary>
    public interface ISubmissionPermissionHandler : IProblemsetResolver
    {
        Task<bool> IsSubmissionViewableAsync(Guid submissionId, Guid? userId);
        Task<bool> IssubmissionInteractableAsync(Guid submissionId, Guid? userId);
    }
}