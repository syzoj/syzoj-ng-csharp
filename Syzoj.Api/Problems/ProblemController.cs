using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Syzoj.Api.Mvc;
using Syzoj.Api.Problems.Standard;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems
{
    [Route("api/problem-standard")]
    public class ProblemController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> Create(
            [FromBody] [BindRequired] ProblemStatement statement,
            [FromServices] Standard.Problem.ProblemProvider provider)
        {
            var problem = await provider.CreateObject(statement);
            return new CustomResponse<Guid>(problem.Id);
        }

        [HttpGet("{problemId}/view")]
        public async Task<ActionResult<CustomResponse<ProblemStatement>>> View(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemId")] Standard.Problem problem)
        {
            return new CustomResponse<ProblemStatement>(await problem.GetProblemStatement());
        }
    }
}