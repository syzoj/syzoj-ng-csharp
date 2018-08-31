using System;
using System.Collections.Generic;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public class ProblemSetServiceProvider : IProblemSetServiceProvider
    {
        private static IDictionary<ProblemSetType, Type> ProblemSetServices = new Dictionary<ProblemSetType, Type>()
        {

        };

        private readonly IServiceProvider serviceProvider;
        public ProblemSetServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IProblemSetService GetProblemSetService(ProblemSet problemSet)
        {
            return (IProblemSetService) serviceProvider.GetService(ProblemSetServices[problemSet.Type]);
        }
    }
}