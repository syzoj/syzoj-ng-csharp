using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Controllers;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public class ProblemResolverProvider : IProblemResolverProvider
    {
        private static IDictionary<ProblemType, Type> ProblemResolvers = new Dictionary<ProblemType, Type>()
        {
            { ProblemType.SyzojLegacyTraditional, typeof(LegacyProblemResolver) },
        };

        private readonly IServiceProvider serviceProvider;
        public ProblemResolverProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IProblemResolver GetProblemResolver(Problem problem)
        {
            return (IProblemResolver) serviceProvider.GetRequiredService(ProblemResolvers[problem.Type]);
        }
    }
}