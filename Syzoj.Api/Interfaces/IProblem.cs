using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Interfaces
{
    public interface IProblem : IObject
    {
        Task<IProblemContract> CreateProblemContract();
    }
}