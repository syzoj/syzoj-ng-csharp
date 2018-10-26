using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces
{
    public interface IProblemSubmission : IObject
    {
        Task<ViewModel> GetSubmissionContent();
        Task<IProblem> GetProblem();
        Task RequestCompleteNotification(ISubmissionCompleteCallback callback);
    }
}