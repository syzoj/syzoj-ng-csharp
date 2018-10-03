using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problemsets
{
    public interface IProblemsetResolverService
    {
        Task<IProblemsetResolver> GetProblemsetResolver(Guid problemsetId);
    }
}