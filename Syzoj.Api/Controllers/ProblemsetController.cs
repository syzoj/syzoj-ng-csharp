using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syzoj.Api.Data;
using System.ComponentModel.DataAnnotations;

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

        private Task<Problemset> GetProblemset(int id)
        {
            return dbContext.Problemsets.Where(psp => psp.Id == id).FirstOrDefaultAsync();
        }

        private Task<ProblemsetProblem> GetProblemsetProblem(int id, string pid)
        {
            return dbContext.ProblemsetProblems.Where(psp => psp.ProblemsetId == id && psp.ProblemsetProblemId == pid).Include(psp => psp.Problem).FirstOrDefaultAsync();
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

            /// <summary>
            /// The type of the problem.
            /// </summary>
            public string ProblemType { get; set; }
        }

        /// <summary>
        /// Gets the list of all problems in the problemset.
        /// </summary>
        /// <param name="id">The ID of the problemset.</param>
        [HttpGet("{id}/problems")]
        public async Task<ActionResult<ProblemsetProblemListResponse>> GetProblemList([FromRoute] int id)
        {
            Problemset ps = await GetProblemset(id);
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
                        ProblemType = x.Problem.ProblemType,
                    }),
            };
        }

        public class PatchProblemIdRequest
        {
            [Required]
            public string NewId { get; set; }
        }
        public class PatchProblemIdResponse
        {
            /// <summary>
            /// Whether the request was successful or not.
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: The problemset is not editable.
            /// - 1003: The problem with specified id doesn't exist.
            /// </summary>
            public int Code { get; set; }
        }
        /// <summary>
        /// Changes problem id for a specified problem.
        /// </summary>
        // TODO: Handle problem id conflict
        [HttpPatch("{id}/problem/{pid}/id")]
        public async Task<ActionResult<PatchProblemIdResponse>> PatchProblemId(int id, string pid, [FromBody] PatchProblemIdRequest request)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new PatchProblemIdResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            IAsyncProblemsetManager problemsetManager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await problemsetManager.IsProblemsetEditableAsync(ps))
            {
                return new PatchProblemIdResponse() {
                    Success = false,
                    Code = 1002,
                };
            }

            ProblemsetProblem problem = await GetProblemsetProblem(id, pid);
            if(problem == null)
            {
                return new PatchProblemIdResponse() {
                    Success = false,
                    Code = 1003,
                };
            }

            problem.ProblemsetProblemId = request.NewId;
            await dbContext.SaveChangesAsync();
            return new PatchProblemIdResponse() {
                Success = true,
                Code = 0,
            };
        }
    }
}