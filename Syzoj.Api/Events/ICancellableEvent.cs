namespace Syzoj.Api.Events
{
    public interface ICancellableEvent : IEvent
    {
        bool IsCancelled { get; }
    }
}