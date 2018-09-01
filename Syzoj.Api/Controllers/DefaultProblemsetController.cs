using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Syzoj.Api.Models.Requests;
using System.Net.Http;
using Syzoj.Api.Services;
using Syzoj.Api.Models.Data;
using MessagePack;
using Syzoj.Api.Models.Runner;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Syzoj.Api.Models.Responses;

namespace Syzoj.Api.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class DefaultProblemsetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IProblemResolverProvider controllerProvider;
        public DefaultProblemsetController(ApplicationDbContext dbContext, IProblemResolverProvider controllerProvider)
        {
            this.dbContext = dbContext;
            this.controllerProvider = controllerProvider;
        }

        /// <summary>
        /// Gets the problem list of public problemset.
        /// </summary>

        [HttpGet("problems")]
        [ProducesResponseType(typeof(ProblemListResponse), 200)]
        public async Task<IActionResult> GetProblemList()
        {
            var problemSet = dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1)
                .Include(psp => psp.Problem)
                .Select(p => new ProblemListEntryResponse() {
                    ProblemSetProblemId = p.ProblemSetProblemId,
                    Title = p.Problem.Title,
                    Type = p.Problem.Type,
                    Submissions = p.Submissions,
                    Accepts = p.Accepts,
                });
            return Ok(new ProblemListResponse() {
                ProblemSet = problemSet
            });
        }

        /// <summary>
        /// Problem-specifc action.
        /// </summary>
        /// <response code="404">
        /// The problem does not exist in the problemset.
        /// </response>
        [HttpGet("problem/{id}/{act?}")]
        [HttpPost("problem/{id}/{act?}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        public async Task<IActionResult> Problem([FromRoute] string id, [FromRoute] string act)
        {
            var problem = await dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1 && psp.ProblemSetProblemId == id)
                .Include(psp => psp.Problem)
                .SingleOrDefaultAsync();
            if(problem == null)
            {
                return NotFound(new BaseResponse() {
                    Status = "Fail",
                    Code = 3,
                    Message = "Problem not found"
                });
            }
            else
            {
                var problemController = controllerProvider.GetProblemResolver(problem.Problem).GetProblemSubmitonlyController(ControllerContext);
                return await problemController.Problem(problem, act);
            }
        }

        /// <summary>
        /// Problem-specifc action.
        /// </summary>
        /// <response code="404">
        /// The problem does not exist in the problemset.
        /// </response>
        [HttpGet("submission/{id}/{act?}")]
        [HttpPost("submission/{id}/{act?}")]
        public async Task<IActionResult> ProblemSubmission([FromRoute] int id, [FromRoute] string act)
        {
            var submission = await dbContext.ProblemSubmissions
                .Where(ps => ps.ProblemSetId == 1 && ps.Id == id)
                .Include(ps => ps.Problem)
                .Include(ps => ps.ProblemSet)
                .SingleOrDefaultAsync();
            if(submission == null)
            {
                return NotFound(new BaseResponse() {
                    Status = "Fail",
                    Code = 3,
                    Message = "Submission not found"
                });
            }
            else
            {
                var problemController = controllerProvider.GetProblemResolver(submission.Problem).GetSubmissionController(ControllerContext);
                return await problemController.Submission(submission, act);
            }
        }
    }
}