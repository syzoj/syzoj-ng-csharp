using System.Threading.Tasks;

namespace Syzoj.Api.Events
{
    public interface IEventHandler
    {
        Task HandleEventAsync(IEvent e);
    }
}