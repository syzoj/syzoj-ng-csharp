using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Mvc;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problems.Standard
{
    [Route("api/problem-standard")]
    [ApiController]
    public class ProblemController : CustomControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> CreateProblem(
            [FromServices] IObjectService objectService,
            [FromServices] ProblemProvider problemProvider,
            [FromServices] ApplicationDbContext dbContext,
            [FromBody] Model.ProblemStatement statement
        )
        {
            var problem = await problemProvider.CreateObject(dbContext, statement);
            return new CustomResponse<Guid>(problem.Id);
        }
    }
}