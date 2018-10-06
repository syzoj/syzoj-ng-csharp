using System;

namespace Syzoj.Api.Events
{
    public interface ISubmissionEvent : IProblemsetEvent, IProblemEvent
    {
        Guid SubmissionId { get; }
    }
}