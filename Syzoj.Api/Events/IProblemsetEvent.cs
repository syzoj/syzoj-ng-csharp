using System;

namespace Syzoj.Api.Events
{
    public interface IProblemsetEvent : IEvent
    {
        Guid ProblemsetId { get; }
    }
}