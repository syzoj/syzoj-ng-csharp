using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Mvc;
using Syzoj.Api.Object;
using Syzoj.Api.Problemsets.Standard;

namespace Syzoj.Api.Problemsets
{
    [Route("api/problemset-standard")]
    [ApiController]
    public class ProblemsetController : ControllerBase
    {
        private readonly IObjectService objectService;

        public ProblemsetController(IObjectService objectService)
        {
            this.objectService = objectService;
        }
        [HttpGet("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> Create([FromServices] Problemset.ProblemsetProvider provider)
        {
            var obj = await provider.CreateObject();
            return new CustomResponse<Guid>(obj.Id);
        }

        public class AddProblemRequest
        {
            public Guid ProblemId { get; set; }
            public string Identifier { get; set; }
        }
        [HttpPost("{problemsetId}/addproblem")]
        public async Task<ActionResult<CustomResponse>> AddProblem(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemsetId")] Problemset problemset,
            [FromBody] AddProblemRequest request)
        {
            var problem = await objectService.GetObject(request.ProblemId);
            if(problem == null)
            {
                ModelState.AddModelError("ProblemId", "Object does not exist.");
                return BadRequest(ModelState);
            }

            var success = await problemset.AddProblem(problem, request.Identifier);
            if(!success)
            {
                ModelState.AddModelError("problemId", "Problem is not supported by this problemset.");
                return BadRequest(ModelState);
            }

            return new CustomResponse();
        }

        [HttpGet("{problemsetId}/problems")]
        public async Task<ActionResult<CustomPagedResponse<Problemset.ProblemSummary>>> Problems(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemsetId")] Problemsets.Standard.Problemset problemset)
        {
            return new CustomPagedResponse<Problemset.ProblemSummary>(await problemset.GetProblems());
        }

        [HttpGet("{problemsetId}/problem/{problemIdentifier}")]
        public async Task<ActionResult<CustomResponse<Problemset.ProblemContent>>> Problem(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemsetId")] Problemsets.Standard.Problemset problemset,
            [FromRoute] [BindRequired] string problemIdentifier)
        {
            var result = await problemset.GetProblem(problemIdentifier);
            if(result == null)
                return NotFound(new CustomResponse<Problemset.ProblemContent>());
            return new CustomResponse<Problemset.ProblemContent>(result);
        }
    }
}