using System.Threading.Tasks;

namespace Syzoj.Api.Interfaces.View
{
    public interface IViewProblemsetContract : IProblemsetContract
    {
        Task OnProblemContentChange();
    }
}