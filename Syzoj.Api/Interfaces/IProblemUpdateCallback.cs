using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces
{
    public interface IProblemUpdateCallback : IObject
    {
        Task<bool> OnProblemUpdated(IProblemContract caller);
    }
}