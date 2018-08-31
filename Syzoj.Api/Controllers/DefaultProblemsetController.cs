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

namespace Syzoj.Api.Controllers
{
    [Route("api/problem")]
    public class DefaultProblemsetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IProblemResolverProvider controllerProvider;
        public DefaultProblemsetController(ApplicationDbContext dbContext, IProblemResolverProvider controllerProvider)
        {
            this.dbContext = dbContext;
            this.controllerProvider = controllerProvider;
        }

        [HttpGet("")]
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
        [HttpGet("{id}/{act?}")]
        public async Task<IActionResult> GetProblem([FromRoute]string id, [FromRoute]string act)
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
                var problemController = controllerProvider.GetProblemResolver(problem.Problem).GetReadonlyController();
                return await problemController.GetProblem(problem, act);
            }
        }

        [HttpPost("{id}/{act?}")]
        public async Task<IActionResult> PostProblem([FromRoute]string id, [FromRoute]string act)
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
                var problemController = controllerProvider.GetProblemResolver(problem.Problem).GetReadonlyController();
                return await problemController.PostProblem(problem, act);
            }
        }
    }
}