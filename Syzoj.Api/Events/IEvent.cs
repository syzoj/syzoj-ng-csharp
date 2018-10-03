using System;

namespace Syzoj.Api.Events
{
    public interface IEvent
    {
        Guid Id { get; }
    }
}