using System;
using System.Threading.Tasks;

namespace Syzoj.Api
{
    /// <summary>
    /// An instance of the interface is responsible for updating information
    /// related to the problemset, invalidate caches, etc.
    /// After an await the information is considered to be acknowledged,
    /// but the manager may perform further actions.
    /// </summary>
    public interface IAsyncProblemsetManager
    {
        /// <summary>
        /// Attaches a problem to the problemset.
        /// </summary>
        Task AttachProblem(Guid problemsetId, Guid problemId, string name);

        /// <summary>
        /// Detaches a problem from the problemset.
        /// </summary>
        Task DetachProblem(Guid problemsetId, Guid problemId);

        /// <summary>
        /// Changes name of a problem in problemset.
        /// </summary>
        Task ChangeProblemName(Guid problemsetId, Guid problemId, string newName);

        /// <summary>
        /// Updates problem statement for specified problem.
        /// </summary>
        Task PutProblem(Guid problemsetId, Guid problemId, object problem);

        /// <summary>
        /// Patches problem statement for specifed problem.
        /// </summary>
        Task PatchProblem(Guid problemsetId, Guid problemId, object problem);

        /// <summary>
        /// Creates a new submission. submissionId should be a fresh new GUID
        /// that didn't exist. This does not verify the submission object nor
        /// performs judge.
        /// </summary>
        Task NewSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission);

        /// <summary>
        /// Updates a submission.
        /// </summary>
        Task PutSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission);

        /// <summary>
        /// Patches a submission.
        /// </summary>
        Task PatchSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission);

        /// <summary>
        /// Performs garbage collection.
        /// </summary>
        Task DoGarbageCollect();

        /// <summary>
        /// Rebuilds the Redis index.
        /// </summary>
        Task RebuildIndex();
    }
}