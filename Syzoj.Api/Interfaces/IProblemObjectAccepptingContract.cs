using System.Threading.Tasks;

namespace Syzoj.Api.Interfaces
{
    public interface IProblemObjectAcceptingContract<TProblemsetContract, TProblemContract> : IProblemObject
        where TProblemsetContract: IProblemsetContract
        where TProblemContract: IProblemContract
    {
        Task<TProblemContract> CreateContract(TProblemsetContract c);
    }
}