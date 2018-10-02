using System.Threading.Tasks;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public interface IStandardProblemStatement : IProblemResolver
    {
        Task<ProblemStatement> GetProblemStatement();
    }
}