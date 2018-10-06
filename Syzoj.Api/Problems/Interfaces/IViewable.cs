using System.Threading.Tasks;

namespace Syzoj.Api.Problems.Interfaces
{
    /// <summary>
    /// Defines the contract for problems that implement their own view.
    /// </summary>
    public interface IViewable : IProblemResolver
    {
        Task<ProblemViewModel> GetProblemView();
    }
}