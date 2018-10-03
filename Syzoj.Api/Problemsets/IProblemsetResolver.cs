using System;
using System.Threading.Tasks;
using Syzoj.Api.Events;

namespace Syzoj.Api.Problemsets
{
    public interface IProblemsetResolver
    {
        Guid Id { get; }
        Task HandleProblemsetEvent(IProblemsetEvent e);
    }
}