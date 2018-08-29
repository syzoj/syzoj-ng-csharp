using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Syzoj.Api.Controllers
{
    [Route("api/problem")]
    public class ProblemController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public ProblemController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                    Submissions = p.Problem.Submissions,
                    Accepts = p.Problem.Accepts,
                });
            return Ok(new {
                Status = "Success",
                ProblemSet = problemSet
            });
        }

        // TODO: Server side rendering
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProblem(string id)
        {
            var problem = await dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1 && psp.ProblemSetProblemId == id)
                .Include(psp => psp.Problem)
                .Select(p => new {
                    Id = p.ProblemSetProblemId,
                    Title = p.Problem.Title,
                    Submissions = p.Problem.Submissions,
                    Accepts = p.Problem.Accepts,
                    Data = p.Problem.Data,
                })
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
                return Ok(new {
                    Status = "Success",
                    Problem = problem
                });
            }
        }
    }
}