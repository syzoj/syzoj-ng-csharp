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

namespace Syzoj.Api.Controllers
{
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

        [HttpGet("problems")]
        public async Task<IActionResult> GetProblemList()
        {
            var problemSet = dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1)
                .Include(psp => psp.Problem)
                .Select(p => new {
                    Id = p.ProblemSetProblemId,
                    Title = p.Problem.Title,
                    Type = p.Problem.Type,
                    Submissions = p.Submissions,
                    Accepts = p.Accepts,
                });
            return Ok(new {
                Status = "Success",
                ProblemSet = problemSet
            });
        }

        // TODO: Server side rendering
        [HttpGet("problem/{id}/{act?}")]
        public async Task<IActionResult> GetProblem([FromRoute] string id, [FromRoute] string act)
        {
            var problem = await dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1 && psp.ProblemSetProblemId == id)
                .Include(psp => psp.Problem)
                .SingleOrDefaultAsync();
            if(problem == null)
            {
                return NotFound(new {
                    Status = "Fail",
                    Message = "Problem not found"
                });
            }
            else
            {
                var problemController = controllerProvider.GetProblemResolver(problem.Problem).GetProblemSubmitonlyController(ControllerContext);
                return await problemController.GetProblem(problem, act);
            }
        }

        [HttpPost("problem/{id}/{act?}")]
        public async Task<IActionResult> PostProblem([FromRoute] string id, [FromRoute] string act)
        {
            var problem = await dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1 && psp.ProblemSetProblemId == id)
                .Include(psp => psp.Problem)
                .SingleOrDefaultAsync();
            if(problem == null)
            {
                return NotFound(new {
                    Status = "Fail",
                    Message = "Problem not found"
                });
            }
            else
            {
                var problemController = controllerProvider.GetProblemResolver(problem.Problem).GetProblemSubmitonlyController(ControllerContext);
                return await problemController.PostProblem(problem, act);
            }
        }

        [HttpGet("submission/{id}/{act?}")]
        public async Task<IActionResult> GetProblemSubmission([FromRoute] int id, [FromRoute] string act)
        {
            var submission = await dbContext.ProblemSubmissions
                .Where(ps => ps.ProblemSetId == 1 && ps.Id == id)
                .Include(ps => ps.Problem)
                .Include(ps => ps.ProblemSet)
                .SingleOrDefaultAsync();
            if(submission == null)
            {
                return NotFound(new {
                    Status = "Fail",
                    Message = "Submission not found"
                });
            }
            else
            {
                var problemController = controllerProvider.GetProblemResolver(submission.Problem).GetSubmissionController(ControllerContext);
                return await problemController.GetSubmission(submission, act);
            }
        }

        [HttpPost("submission/{id}/{act?}")]
        public async Task<IActionResult> PostProblemSubmission([FromRoute] int id, [FromRoute] string act)
        {
            var submission = await dbContext.ProblemSubmissions
                .Where(ps => ps.ProblemSetId == 1 && ps.Id == id)
                .Include(ps => ps.Problem)
                .Include(ps => ps.ProblemSet)
                .SingleOrDefaultAsync();
            if(submission == null)
            {
                return NotFound(new {
                    Status = "Fail",
                    Message = "Submission not found"
                });
            }
            else
            {
                var problemController = controllerProvider.GetProblemResolver(submission.Problem).GetSubmissionController(ControllerContext);
                return await problemController.PostSubmission(submission, act);
            }
        }
    }
}