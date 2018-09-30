using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetManagerProvider
    {
        Task<IProblemsetManager> GetProblemsetManager(Guid problemsetId);
    }
}