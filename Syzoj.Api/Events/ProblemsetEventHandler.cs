using System.Threading.Tasks;
using Syzoj.Api.Problemsets;

namespace Syzoj.Api.Events
{
    public class ProblemsetEventHandler : IEventHandler
    {
        private readonly IProblemsetResolverService Service;

        public ProblemsetEventHandler(IProblemsetResolverService service)
        {
            this.Service = service;
        }

        public async Task HandleEventAsync(IEvent e)
        {
            if(e is IProblemsetEvent pse)
            {
                IProblemsetResolver ps = await Service.GetProblemsetResolver(e.Id);
                await ps?.HandleProblemsetEvent(pse);
            }
        }
    }
}