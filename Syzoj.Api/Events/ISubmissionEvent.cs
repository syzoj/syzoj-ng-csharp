using System;

namespace Syzoj.Api.Events
{
    public interface ISubmissionEvent : IProblemsetEvent
    {
        Guid SubmissionId { get; }
    }
}