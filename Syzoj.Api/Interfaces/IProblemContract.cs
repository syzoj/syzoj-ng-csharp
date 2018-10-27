using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces
{
    public interface IProblemContract : IObject
    {
        Task<ViewModel> GetProblemContent();
        Task RequestUpdateNotification(IProblemUpdateCallback callback);
        Task CancelContract();
        Task<IProblemSubmission> ClaimSubmission(string token);
    }
}