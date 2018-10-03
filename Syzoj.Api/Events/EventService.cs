using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syzoj.Api.Events
{
    public class EventService : IEventService
    {
        private readonly IEnumerable<IEventHandler> EventHandlers;

        public EventService(IEnumerable<IEventHandler> EventHandlers)
        {
            this.EventHandlers = EventHandlers;
        }

        public async Task HandleEventAsync(IEvent ev)
        {
            foreach(var handler in EventHandlers)
                await handler.HandleEventAsync(ev);
        }
    }
}