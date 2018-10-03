using System;

namespace Syzoj.Api.Events
{
    public interface IProblemEvent : IEvent
    {
        Guid ProblemId { get; }
    }
}