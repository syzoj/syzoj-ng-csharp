using System;
using System.Collections.Generic;

namespace Syzoj.Api
{
    public class ProblemsetManagerProvider
    {
        private static Dictionary<string, Type> providers = new Dictionary<string, Type>()
        {
            { "debug", typeof(DebugProblemsetManager) }
        };
        private readonly IServiceProvider serviceProvider;

        public ProblemsetManagerProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IAsyncProblemsetManager GetProblemsetManager(string Name)
        {
            return (IAsyncProblemsetManager) serviceProvider.GetService(providers.GetValueOrDefault(Name));
        }
    }
}