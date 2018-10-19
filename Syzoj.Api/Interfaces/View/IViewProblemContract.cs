using System.Threading.Tasks;

namespace Syzoj.Api.Interfaces.View
{
    public interface IViewProblemContract : IProblemContract
    {
        Task<string> GetProblemDefaultTitle();
        Task<ViewModel> GetProblemContent();
    }
}