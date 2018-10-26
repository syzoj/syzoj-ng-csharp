using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces
{
    public interface ISubmissionCompleteCallback : IObject
    {
        Task<bool> OnSubmissionCompleted(IProblemSubmission caller);
    }
}