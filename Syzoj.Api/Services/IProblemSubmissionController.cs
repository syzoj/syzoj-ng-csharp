using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemSubmissionController
    {
        Task<IActionResult> GetSubmission(ProblemSubmission submission, string action);
        Task<IActionResult> PostSubmission(ProblemSubmission submission, string action);
    }
}