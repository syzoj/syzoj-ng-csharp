using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syzoj.Api.Data;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using System;

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
            public Guid ProblemId { get; set; }

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
        /// <param name="sort">Can be either `asc` or `desc`, specifying how the list shoud be sorted.</param>
        /// <param name="key">The sorting key.</param>
        /// <param name="page">Page number.</param>
        // TODO: Implement sorting and pagination
        [HttpGet("{id}/problems")]
        public async Task<ActionResult<ProblemsetProblemListResponse>> GetProblemList([FromRoute] int id, [FromQuery] string sort, [FromQuery] string key, [FromQuery] int page)
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

        public class GetProblemResponse
        {
            /// <summary>
            /// Whether the request was successful or not.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: The problem is not viewable.
            /// - 1003: The problem with specified id doesn't exist.
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// The global problem ID for the problem.
            /// </summary>
            public Guid ProblemId { get; set; }

            /// <summary>
            /// The problem identifier in this problemset.
            /// </summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>
            /// The problem's title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// The problem's type.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// The content of the problem. The exact model depends on the problem's type.
            /// </summary>
            public object Content { get; set; }

            /// <summary>
            /// Whether the current user can submit to the problem.
            /// </summary>
            public bool Submittable { get; set; }
        }

        /// <summary>
        /// Gets problem statement for a problem.
        /// </summary>
        [HttpGet("{id}/problem/{pid}")]
        public async Task<ActionResult<GetProblemResponse>> GetProblem(int id, string pid)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new GetProblemResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            IAsyncProblemsetManager problemsetManager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            ProblemsetProblem problem = await GetProblemsetProblem(id, pid);
            if(problem == null)
            {
                return new GetProblemResponse() {
                    Success = false,
                    Code = 1003,
                };
            }
            if(!await problemsetManager.IsProblemViewableAsync(ps, problem))
            {
                return new GetProblemResponse() {
                    Success = false,
                    Code = 1002,
                };
            }

            var content = MessagePackSerializer.Deserialize<object>(problem.Problem.Statement);
            return new GetProblemResponse() {
                Success = true,
                Code = 0,
                ProblemId = problem.Problem.Id,
                ProblemsetProblemId = problem.ProblemsetProblemId,
                Title = problem.Problem.Title,
                Type = problem.Problem.ProblemType,
                Content = content,
                Submittable = problem.Problem.IsSubmittable && await problemsetManager.IsProblemSubmittableAsync(ps, problem)
            };
        }

        public class LoadProblemResponse
        {
            /// <summary>
            /// Whether the request was successful or not.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: The problem is not editable.
            /// - 1003: The problem with specified id doesn't exist.
            /// </summary>
            public int Code { get; set; }
        }
        /// <summary>
        /// Loads the problem statement from data folder.
        /// </summary>
        [HttpPost("{id}/problem/{pid}/load")]
        public async Task<ActionResult<LoadProblemResponse>> LoadProblem(int id, string pid)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new LoadProblemResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            ProblemsetProblem problem = await GetProblemsetProblem(id, pid);
            if(problem == null)
            {
                return new LoadProblemResponse() {
                    Success = false,
                    Code = 1003,
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await manager.IsProblemEditableAsync(ps, problem))
            {
                return new LoadProblemResponse() {
                    Success = false,
                    Code = 1002,
                };
            }
            IAsyncProblemParser parser = problemParserProvider.GetParser(problem.Problem.ProblemType);
            await parser.ParseProblemAsync(problem.Problem);
            return new LoadProblemResponse() {
                Success = true,
                Code = 0,
            };
        }

        public class SubmitProblemRequest
        {
            /// <summary>
            /// The contents of the submission.
            /// </summary>
            public object Content { get; set; }
        }
        public class SubmitProblemResponse
        {
            /// <summary>
            /// Whether the request was successful or not.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: User is not allowed to submit to the problem.
            /// - 1003: The problem with specified id doesn't exist.
            /// - 1004: The problem is not submittable.
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// The id of the new submission.
            /// </summary>
            public int SubmissionId { get; set; }
        }
        /// <summary>
        /// Creates a new submission.
        /// </summary>
        [HttpPost("{id}/problem/{pid}/submit")]
        public async Task<ActionResult<SubmitProblemResponse>> SubmitProblem([FromRoute] int id, [FromRoute] string pid, [FromBody] SubmitProblemRequest request)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            ProblemsetProblem problem = await GetProblemsetProblem(id, pid);
            if(problem == null)
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    Code = 1003,
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await manager.IsProblemSubmittableAsync(ps, problem))
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    Code = 1002,
                };
            }
            if(!problem.Problem.IsSubmittable)
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    Code = 1004,
                };
            }
            var submission = new Submission() {
                ProblemId = problem.ProblemId,
                ProblemsetId = problem.ProblemsetId,
            };
            dbContext.Submissions.Add(submission);
            await dbContext.SaveChangesAsync();
            IAsyncProblemParser parser = problemParserProvider.GetParser(problem.Problem.ProblemType);
            await parser.HandleSubmissionAsync(problem.Problem, submission, request.Content);
            return new SubmitProblemResponse() {
                Success = true,
                Code = 0,
                SubmissionId = submission.Id,
            };
        }

        public class GetSubmissionsResponse
        {
            /// <summary>
            /// Whether the request was successful or not.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: The problemset's submissions are not viewable.
            /// - 1003: The problem doesn't exist.
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// The list of submissions.
            /// </summary>
            public IEnumerable<SubmissionSummaryEntry> Submissions { get; set; }
        }
        public class SubmissionSummaryEntry
        {
            /// <summary>The global id of the corresponding problem.</summary>
            public Guid ProblemId { get; set; }

            /// <summary>The ID of the corresponding problem.</summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>The ID of the submission.</summary>
            public int SubmissionId { get; set; }

            /// <summary>
            /// Problem Type specific object describing the submission.
            /// </summary>
            public object Content { get; set; }
        }
        /// <summary>
        /// Gets all submissions in the problemset.
        /// </summary>
        [HttpGet("{id}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetSubmissions(int id)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await manager.IsSubmissionListVisibleAsync(ps))
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    Code = 1002,
                };
            }

            var submissions = dbContext.Entry(ps)
                .Collection(x => x.Submissions)
                .Query()
                .Select(s => new SubmissionSummaryEntry() {
                    ProblemId = s.ProblemId,
                    ProblemsetProblemId = s.ProblemsetProblem.ProblemsetProblemId,
                    SubmissionId = s.Id,
                    Content = MessagePackSerializer.Deserialize<object>(s.Summary),
                });
            return new GetSubmissionsResponse() {
                Success = true,
                Code = 0,
                Submissions = submissions,
            };
        }

        /// <summary>
        /// Gets all submissions for a specific problem in the problemset.
        /// </summary>
        [HttpGet("{id}/problem/{pid}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetProblemSubmissions(int id, string pid)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await manager.IsSubmissionListVisibleAsync(ps))
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    Code = 1002,
                };
            }
            ProblemsetProblem problem = await GetProblemsetProblem(id ,pid);
            if(problem == null)
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    Code = 1003,
                };
            }
            var submissions = dbContext.Entry(problem)
                .Collection(x => x.Submissions)
                .Query()
                .Select(s => new SubmissionSummaryEntry() {
                    ProblemId = s.ProblemId,
                    ProblemsetProblemId = s.ProblemsetProblem.ProblemsetProblemId,
                    SubmissionId = s.Id,
                    Content = MessagePackSerializer.Deserialize<object>(s.Summary),
                });
            return new GetSubmissionsResponse() {
                Success = true,
                Code = 0,
                Submissions = submissions,
            };
        }

        public class GetSubmissionResponse
        {
            /// <summary>
            /// Whether the request was successful or not.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// An integer describing the status of the query. Possible values are:
            /// - 0: The request was successful.
            /// - 1001: The problemset doesn't exist.
            /// - 1002: The submission is not viewable.
            /// - 1003: The submission doesn't exist.
            /// </summary>
            public int Code { get; set; }

            /// <summary>The global id of the corresponding problem.</summary>
            public Guid ProblemId { get; set; }

            /// <summary>The ID of the corresponding problem.</summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>The ID of the submission.</summary>
            public int SubmissionId { get; set; }

            /// <summary>
            /// Problem Type specific object describing the submission.
            /// </summary>
            public object Content { get; set; }
        }
        /// <summary>
        /// Gets a specified submission.
        /// </summary>
        [HttpGet("{id}/submission/{sid}")]
        public async Task<ActionResult<GetSubmissionResponse>> GetSubmission(int id, int sid)
        {
            Problemset ps = await GetProblemset(id);
            if(ps == null)
            {
                return new GetSubmissionResponse() {
                    Success = false,
                    Code = 1001,
                };
            }
            var submission = await dbContext.Entry(ps)
                .Collection(x => x.Submissions)
                .Query()
                .Where(s => s.Id == sid)
                .Select(s => new { Submission = s, ProblemsetProblemId = s.ProblemsetProblem.ProblemsetProblemId })
                .FirstOrDefaultAsync();
            if(submission == null)
            {
                return new GetSubmissionResponse() {
                    Success = false,
                    Code = 1003,
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(ps.Type);
            if(!await manager.IsSubmissionViewableAsync(ps, submission.Submission))
            {
                return new GetSubmissionResponse() {
                    Success = false,
                    Code = 1002,
                };
            }
            return new GetSubmissionResponse() {
                Success = true,
                Code = 0,
                ProblemId = submission.Submission.ProblemId,
                ProblemsetProblemId = submission.ProblemsetProblemId,
                SubmissionId = submission.Submission.Id,
                Content = MessagePackSerializer.Deserialize<object>(submission.Submission.Content)
            };
        }
    }
}