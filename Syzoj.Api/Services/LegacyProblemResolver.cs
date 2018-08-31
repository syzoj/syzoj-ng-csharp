using Syzoj.Api.Controllers;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Syzoj.Api.Services
{
    public class LegacyProblemResolver : IProblemResolver
    {
        private readonly IServiceProvider serviceProvider;
        public LegacyProblemResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IProblemController GetProblemFullController()
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.SetFullController();
            return controller;
        }

        public IProblemController GetProblemSubmitonlyController()
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.SetSubmitonlyController();
            return controller;
        }

        public IProblemController GetProblemViewonlyController()
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.SetViewonlyController();
            return controller;
        }

        public IProblemSubmissionController GetSubmissionController()
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            return controller;
        }
    }
}