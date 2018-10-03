using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problemsets
{
    public interface IProblemsetResolverProvider
    {
        string ProblemsetType { get; }
        Task<IProblemsetResolver> GetProblemsetResolver(IServiceProvider serviceProvider, Guid problemsetId);
    }
}