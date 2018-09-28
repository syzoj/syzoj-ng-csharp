using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetManagerFactory
    {
        Task<IProblemsetManager> GetProblemsetManager(Guid problemsetId);
    }
}