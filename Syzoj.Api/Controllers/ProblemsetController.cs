using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syzoj.Api.Data;

namespace Syzoj.Api.Controllers
{
    [Route("api/problemset")]
    public class ProblemsetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ProblemParserProvider problemParserProvider;
        private readonly ProblemsetManagerProvider problemsetManagerProvider;
        public ProblemsetController(ApplicationDbContext dbContext, ProblemParserProvider problemParserProvider, ProblemsetManagerProvider problemsetManagerProvider)
        {
            this.dbContext = dbContext;
            this.problemParserProvider = problemParserProvider;
            this.problemsetManagerProvider = problemsetManagerProvider;
        }

        public class ProblemsetProblemListResponse
        {
            /// <summary>
            /// Whether the query was successful or not.
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: The problemset is not accessible.
            /// </summary>
            public int Code { get; set; }
            /// <summary>
            /// The list of problems from the problemset.
            /// </summary>
            public IEnumerable<ProblemsetProblemSummaryEntry> Problems { get; set; }
        }
        public class ProblemsetProblemSummaryEntry
        {
            /// <summary>
            /// The global problem ID for the problem.
            /// </summary>
            public int ProblemId { get; set; }
            /// <summary>
            /// The problem identifier in this problemset.
            /// </summary>
            public string ProblemsetProblemId { get; set; }
            /// <summary>
            /// The title of the problem.
            /// </summary>
            public string Title { get; set; }
        }

        /// <summary>
        /// Gets the list of all problems in the problemset.
        /// </summary>
        /// <param name="id">The ID of the problemset.</param>
        [HttpGet("{id}/problems")]
        public async Task<ActionResult<ProblemsetProblemListResponse>> GetProblemList([FromRoute] int id)
        {
            Problemset ps = await dbContext.Problemsets.Where(psp => psp.Id == id).FirstOrDefaultAsync();
            if(ps == null)
            {
                return new ProblemsetProblemListResponse() {
                    Success = false,
                    Code = 1001,
                };
            }

            IAsyncProblemsetManager problemsetManager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await problemsetManager.IsProblemListVisibleAsync(ps))
            {
                return new ProblemsetProblemListResponse() {
                    Success = false,
                    Code = 1002,
                };
            }

            return new ProblemsetProblemListResponse() {
                Success = true,
                Code = 0,
                Problems = dbContext.Entry(ps)
                    .Collection(x => x.ProblemsetProblems)
                    .Query()
                    .Select(x => new ProblemsetProblemSummaryEntry() {
                        ProblemId = x.Problem.Id,
                        ProblemsetProblemId = x.ProblemsetProblemId,
                        Title = x.Problem.Title,
                    }),
            };
        }
    }
}