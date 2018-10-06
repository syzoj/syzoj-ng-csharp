using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Mvc;
using Syzoj.Api.Problems;

namespace Syzoj.Api.Problemsets.Default
{
    [Route("problemset/defualt/{problemsetId}")]
    public class DefaultProblemsetController : CustomControllerBase
    {
        private readonly IProblemsetResolverService problemsetResolverService;
        private readonly IProblemResolverService problemResolverService;
        private readonly ApplicationDbContext dbContext;

        public DefaultProblemsetController(IProblemsetResolverService problemsetResolverService, IProblemResolverService problemResolverService, ApplicationDbContext dbContext)
        {
            this.problemsetResolverService = problemsetResolverService;
            this.problemResolverService = problemResolverService;
            this.dbContext = dbContext;
        }

        public class DefaultProblemsetListEntry
        {
            public Guid ProblemId { get; set; }
            public string ProblemsetProblemId { get; set; }
            public string Title { get; set; }
        }
        [HttpGet("list")]
        public async Task<ActionResult<CustomPagedResponse<DefaultProblemsetListEntry>>> List(Guid problemsetId, string sort = "problemsetProblemId_asc", int Page = 0, [Range(1, 100)] int PerPage = 50)
        {
            var problemsetResolver = await problemsetResolverService.GetProblemsetResolver(problemsetId);
            if(problemsetResolver is DefaultProblemsetResolver defaultProblemsetResolver)
            {
                var problemsetModel = await dbContext.Problemsets.FindAsync(problemsetId);
                var problemsetProblems = dbContext.Entry(problemsetModel)
                    .Collection(p => p.ProblemsetProblems)
                    .Query();
                switch(sort)
                {
                    case "problemsetProblemId_asc":
                        problemsetProblems = problemsetProblems.OrderBy(psp => psp.ProblemsetProblemId);
                        break;
                    case "problemsetProblemId_desc":
                        problemsetProblems = problemsetProblems.OrderByDescending(psp => psp.ProblemsetProblemId);
                        break;
                    default:
                        goto case "problemsetProblemId_asc";
                }
                int totalCount = problemsetProblems.Count();
                problemsetProblems = problemsetProblems.Skip(Page * PerPage).Take(PerPage);
                var list = await problemsetProblems.Select(psp => new DefaultProblemsetListEntry() {
                    ProblemId = psp.ProblemId,
                    ProblemsetProblemId = psp.ProblemsetProblemId,
                    Title = psp.Title,
                }).ToListAsync();
                return new CustomPagedResponse<DefaultProblemsetListEntry>(list) {
                    PageInfo = new PaginationInfo() {
                        TotalCount = totalCount,
                        Page = Page,
                        PerPage = PerPage,
                    },
                };
            }
            else
            {
                return BadRequest();
            }
        }

        public class AddProblemRequest
        {
            public Guid problemId { get; set; }
            [Required]
            public string ProblemsetProblemId { get; set; }
            [MinLength(1)]
            [MaxLength(256)]
            public string Title { get; set; }
        }
        [HttpPost("add")]
        public async Task<ActionResult<CustomResponse<bool>>> Add(Guid problemsetId, [FromBody] AddProblemRequest request)
        {
            var problemsetResolver = await problemsetResolverService.GetProblemsetResolver(problemsetId);
            if(problemsetResolver is DefaultProblemsetResolver defaultProblemsetResolver)
            {
                var problemResolver = await problemResolverService.GetProblemResolver(request.problemId);
                if(problemResolver == null)
                {
                    ModelState.AddModelError("problemId", "problemId is invalid.");
                }
                if(!await defaultProblemsetResolver.IsProblemAcceptable(problemResolver) || !await problemResolver.IsProblemsetAcceptable(defaultProblemsetResolver))
                {
                    ModelState.AddModelError("problemId", "The problem type is not acceptable by the problemset.");
                }
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                var entry = new ProblemsetProblem() {
                    ProblemsetId = problemsetResolver.Id,
                    ProblemsetProblemId = request.ProblemsetProblemId,
                    Title = request.Title,
                };
                dbContext.ProblemsetProblems.Add(entry);
                await dbContext.SaveChangesAsync();
                return new CustomResponse<bool>(true);
            }
            else
            {
                return BadRequest();
            }
        }
        
    }
}