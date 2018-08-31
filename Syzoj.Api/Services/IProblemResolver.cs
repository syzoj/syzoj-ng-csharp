using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Controllers;

namespace Syzoj.Api.Services
{
    public interface IProblemResolver
    {
        IProblemController GetProblemFullController(ControllerContext context);
        IProblemController GetProblemSubmitonlyController(ControllerContext context);
        IProblemController GetProblemViewonlyController(ControllerContext context);
        IProblemSubmissionController GetSubmissionController(ControllerContext context);
    }
}