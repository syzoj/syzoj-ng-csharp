using System;
using System.Threading.Tasks;
using Syzoj.Api.Events;
using Syzoj.Api.Problems;

namespace Syzoj.Api.Problemsets
{
    public interface IProblemsetResolver
    {
        Guid Id { get; }
        Task<bool> IsProblemAcceptable(IProblemResolver problemResolver);
    }
}