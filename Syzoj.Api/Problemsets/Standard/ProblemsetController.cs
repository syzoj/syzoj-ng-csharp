using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Mvc;
using Syzoj.Api.Object;
using Syzoj.Api.Problemsets.Standard;

namespace Syzoj.Api.Problemsets.Standard
{
    [Route("api/problemset-standard")]
    [ApiController]
    public class ProblemsetController : CustomControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ProblemsetProvider problemsetProvider;
        private readonly IObjectService objectService;

        public ProblemsetController(ApplicationDbContext dbContext, ProblemsetProvider problemsetProvider, IObjectService objectService)
        {
            this.dbContext = dbContext;
            this.problemsetProvider = problemsetProvider;
            this.objectService = objectService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> Create()
        {
            var problemset = await problemsetProvider.CreateObject(dbContext);
            return new CustomResponse<Guid>(problemset.Id);
        }

        public class AddProblemRequest
        {
            public Guid ProblemId { get; set; }
            [Required]
            public string Identifier { get; set; }
        }
        [HttpPost("{problemsetId}/add")]
        public async Task<ActionResult<CustomResponse>> Add(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemsetId")] Problemset problemset,
            [FromBody] AddProblemRequest request
        )
        {
            var problem = await objectService.GetObject(dbContext, request.ProblemId) as IProblem;
            if(problem == null)
            {
                ModelState.AddModelError("ProblemId", "Problem does not exist.");
                return BadRequest(ModelState);
            }
            await problemset.AddProblem(problem, request.Identifier);
            return new CustomResponse();
        }

        [HttpGet("{problemsetId}/list")]
        public async Task<ActionResult<CustomPagedResponse<Problemset.ProblemSummary>>> List(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemsetId")] Problemset problemset
        )
        {
            var problems = await problemset.GetProblems();
            return new CustomPagedResponse<Problemset.ProblemSummary>(problems);
        }

        [HttpGet("{problemsetId}/view/{id}")]
        public async Task<ActionResult<CustomResponse<ViewModel>>> View(
            [FromRoute] [BindRequired] [ModelBinder(Name = "problemsetId")] Problemset problemset,
            [FromRoute] string id
        )
        {
            var content = await problemset.GetProblemContent(id);
            return new CustomResponse<ViewModel>(content);
        }
    }
}