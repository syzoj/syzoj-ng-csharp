using Syzoj.Api.Controllers;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Syzoj.Api.Services
{
    public class LegacyProblemResolver : IProblemResolver
    {
        private readonly IServiceProvider serviceProvider;
        public LegacyProblemResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IProblemController GetProblemFullController(ControllerContext context)
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.ControllerContext = context;
            controller.SetFullController();
            return controller;
        }

        public IProblemController GetProblemSubmitonlyController(ControllerContext context)
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.ControllerContext = context;
            controller.SetSubmitonlyController();
            return controller;
        }

        public IProblemController GetProblemViewonlyController(ControllerContext context)
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.ControllerContext = context;
            controller.SetViewonlyController();
            return controller;
        }

        public IProblemSubmissionController GetSubmissionController(ControllerContext context)
        {
            var controller = (LegacyProblemController) serviceProvider.GetRequiredService(typeof(LegacyProblemController));
            controller.ControllerContext = context;
            return controller;
        }
    }
}