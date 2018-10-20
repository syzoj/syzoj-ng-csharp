using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces.View
{
    public interface IViewProblemContract : IObject
    {
        Task<ViewModel> GetProblemStatement();
        Task<string> GetProblemDefaultTitle();
        Task RequestNotificationOnUpdate();
        Task CancelContract();
    }
}