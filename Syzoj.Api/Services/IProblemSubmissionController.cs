using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemSubmissionController
    {
        Task<IActionResult> Submission(ProblemSubmission submission, string action);
    }
}