using System.Threading.Tasks;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problems
{
    public interface IProblemAcceptingContract<IProblemsetContract, IProblemContract> : IObject
        where IProblemsetContract : IObject
        where IProblemContract : IObject
    {
        Task<IProblemContract> BuildContract(IProblemsetContract contract);
    }
}