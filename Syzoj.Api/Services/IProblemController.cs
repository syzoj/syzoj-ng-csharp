using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemController
    {
        /// <remark>
        /// The <see cref="Models.Data.ProblemSetProblem" /> instance supplied here must
        /// have <see cref="Models.Data.Problem" /> included.
        /// </remark>
        Task<IActionResult> Problem(ProblemSetProblem psp, string action);
    }
}