using System;
using System.Threading.Tasks;
using Syzoj.Api.Events;

namespace Syzoj.Api.Problemsets
{
    public abstract class ProblemsetResolverBase : IProblemsetResolver
    {
        private readonly IServiceProvider ServiceProvider;

        public Guid Id { get; }

        public ProblemsetResolverBase(IServiceProvider serviceProvider, Guid problemsetId)
        {
            this.ServiceProvider = serviceProvider;
            this.Id = problemsetId;
        }

        public virtual Task HandleProblemsetEvent(IProblemsetEvent e)
        {
            return Task.CompletedTask;
        }
    }
}