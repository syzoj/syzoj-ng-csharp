using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Filters;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    [ApiController]
    [ProblemController]
    public class StandardProblemController : ControllerBase
    {
        [HttpGet("standard/view")]
        public async Task<ActionResult<ProblemStatement>> View([FromRoute] IProblemsetManager problemset, [FromRoute] IProblemResolver problem)
        {
            if(problem is IStandardProblemStatement problemWithStatement)
            {
                return await problemWithStatement.GetProblemStatement();
            }
            else
            {
                return Ok("Not supported");
            }
        }
    }
}