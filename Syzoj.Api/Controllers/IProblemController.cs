using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Controllers
{
    public interface IProblemController
    {
        Task<IActionResult> GetProblem(ProblemSetProblem psp, string action);
        Task<IActionResult> PostProblem(ProblemSetProblem psp, string action);
        Task<IActionResult> GetSubmission(ProblemSubmission submission, string action);
        Task<IActionResult> PostSubmission(ProblemSubmission submission, string action);
    }
}