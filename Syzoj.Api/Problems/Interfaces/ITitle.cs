using System.Threading.Tasks;

namespace Syzoj.Api.Problems.Interfaces
{
    public interface ITitle : IProblemResolver
    {
        Task<string> GetDefaultTitle();
    }
}