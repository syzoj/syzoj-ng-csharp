using System.Threading.Tasks;

namespace Syzoj.Api.Events
{
    public interface IEventService
    {
        Task HandleEventAsync(IEvent ev);
    }
}