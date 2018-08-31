using System;
using System.Collections.Generic;
using Syzoj.Api.Controllers;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public class ProblemResolverProvider : IProblemResolverProvider
    {
        private static IDictionary<ProblemType, Type> ProblemResolvers = new Dictionary<ProblemType, Type>()
        {

        };

        private readonly IServiceProvider serviceProvider;
        public ProblemResolverProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IProblemResolver GetProblemResolver(Problem problem)
        {
            return (IProblemResolver) serviceProvider.GetService(ProblemResolvers[problem.Type]);
        }
    }
}