using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces.View
{
    public interface IViewProblemsetContract : IObject
    {
        Task OnProblemUpdated();
    }
}