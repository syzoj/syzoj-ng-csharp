using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Controllers
{
    public abstract class ProblemControllerBase : ControllerBase, IProblemController
    {
        public abstract Task<IActionResult> GetProblem(ProblemSetProblem psp, string action);
        public abstract Task<IActionResult> GetSubmission(ProblemSubmission submission, string action);
        public abstract Task<IActionResult> PostProblem(ProblemSetProblem psp, string action);
        public abstract Task<IActionResult> PostSubmission(ProblemSubmission submission, string action);
    }
}