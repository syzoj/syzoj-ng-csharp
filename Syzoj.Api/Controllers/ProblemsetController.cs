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
using Z.EntityFramework.Plus;

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

        public class ProblemsetProblemListResponse : BaseResponse
        {
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
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="sort">Can be either `asc` or `desc`, specifying how the list shoud be sorted.</param>
        /// <param name="key">The sorting key.</param>
        /// <param name="page">Page number.</param>
        // TODO: Implement sorting and pagination
        [HttpGet("{Id}/problems")]
        public async Task<ActionResult<ProblemsetProblemListResponse>> GetProblemList([FromRoute] Guid Id, [FromQuery] string sort, [FromQuery] string key, [FromQuery] int page)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new ProblemsetProblemListResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            IAsyncProblemsetManager problemsetManager = problemsetManagerProvider.GetProblemsetManager(type);
            if(!await problemsetManager.IsProblemListVisibleAsync(Id))
            {
                return new ProblemsetProblemListResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotViewable",
                };
            }

            return new ProblemsetProblemListResponse() {
                Success = true,
                Problems = dbContext.ProblemsetProblems
                    .Where(psp => psp.ProblemsetId == Id)
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
        public class PatchProblemIdResponse : BaseResponse
        {
        }
        /// <summary>
        /// Changes problem id for a specified problem.
        /// </summary>
        // TODO: Handle problem id conflict
        [HttpPatch("{Id}/problem/{pid}/id")]
        public async Task<ActionResult<PatchProblemIdResponse>> PatchProblemId(Guid Id, string pid, [FromBody] PatchProblemIdRequest request)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new PatchProblemIdResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            IAsyncProblemsetManager problemsetManager = problemsetManagerProvider.GetProblemsetManager(type);
            if(!await problemsetManager.IsProblemsetEditableAsync(Id))
            {
                return new PatchProblemIdResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotEditable",
                };
            }

            var cnt = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == Id && psp.ProblemsetProblemId == pid)
                .UpdateAsync(psp => new ProblemsetProblem() { ProblemsetProblemId = request.NewId });
            if(cnt == 0)
            {
                return new PatchProblemIdResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound",
                };
            }

            return new PatchProblemIdResponse() {
                Success = true,
            };
        }

        public class GetProblemResponse : BaseResponse
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
        [HttpGet("{Id}/problem/{pid}")]
        public async Task<ActionResult<GetProblemResponse>> GetProblem(Guid Id, string pid)
        {
            var psType = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(psType == null)
            {
                return new GetProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            IAsyncProblemsetManager problemsetManager = problemsetManagerProvider.GetProblemsetManager(psType);
            var problemId = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == Id && psp.ProblemsetProblemId == pid)
                .Select(psp => psp.ProblemId)
                .FirstOrDefaultAsync();
            if(problemId == default(Guid))
            {
                return new GetProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound",
                };
            }
            if(!await problemsetManager.IsProblemViewableAsync(Id, problemId))
            {
                return new GetProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotViewable",
                };
            }

            var (title, content, problemType, isSubmittable) = await dbContext.Problems
                .Where(p => p.Id == problemId)
                .Select(p => ValueTuple.Create(p.Title, MessagePackSerializer.Deserialize<object>(p.Statement), p.ProblemType, p.IsSubmittable))
                .FirstOrDefaultAsync();
            return new GetProblemResponse() {
                Success = true,
                ProblemId = problemId,
                ProblemsetProblemId = pid,
                Title = title,
                Type = problemType,
                Content = content,
                Submittable = isSubmittable && await problemsetManager.IsProblemSubmittableAsync(Id, problemId)
            };
        }

        public class LoadProblemResponse : BaseResponse
        {
        }
        /// <summary>
        /// Loads the problem statement from data folder.
        /// </summary>
        [HttpPost("{Id}/problem/{pid}/load")]
        public async Task<ActionResult<LoadProblemResponse>> LoadProblem(Guid Id, string pid)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new LoadProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(type);
            var (problemType, problemId) = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == Id && psp.ProblemsetProblemId == pid)
                .Select(psp => ValueTuple.Create(psp.Problem.ProblemType, psp.ProblemId))
                .FirstOrDefaultAsync();
            if(problemId == default(Guid))
            {
                return new LoadProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound"
                };
            }
            if(!await manager.IsProblemEditableAsync(Id, problemId))
            {
                return new LoadProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotEditable",
                };
            }
            IAsyncProblemParser parser = problemParserProvider.GetParser(problemType);
            await parser.ParseProblemAsync(problemId);
            return new LoadProblemResponse() {
                Success = true,
            };
        }

        public class SubmitProblemRequest
        {
            /// <summary>
            /// The contents of the submission.
            /// </summary>
            public object Content { get; set; }
        }
        public class SubmitProblemResponse : BaseResponse
        {
            /// <summary>
            /// The id of the new submission.
            /// </summary>
            public Guid SubmissionId { get; set; }
        }
        /// <summary>
        /// Creates a new submission.
        /// </summary>
        [HttpPost("{Id}/problem/{pid}/submit")]
        public async Task<ActionResult<SubmitProblemResponse>> SubmitProblem([FromRoute] Guid Id, [FromRoute] string pid, [FromBody] SubmitProblemRequest request)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            var problemId = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == Id && psp.ProblemsetProblemId == pid)
                .Select(psp => psp.ProblemId)
                .FirstOrDefaultAsync();
            if(problemId == null)
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound",
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(type);
            var (isSubmittable, problemType) = await dbContext.Problems
                .Where(p => p.Id == problemId)
                .Select(p => ValueTuple.Create(p.IsSubmittable, p.ProblemType))
                .FirstOrDefaultAsync();
            if(!isSubmittable)
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotSubmittable",
                };
            }
            if(!await manager.IsProblemSubmittableAsync(Id, problemId))
            {
                return new SubmitProblemResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotSubmittable",
                };
            }
            var submission = new Submission() {
                Id = Guid.NewGuid(),
                ProblemId = problemId,
                ProblemsetId = Id,
            };
            dbContext.Submissions.Add(submission);
            await dbContext.SaveChangesAsync();
            IAsyncProblemParser parser = problemParserProvider.GetParser(problemType);
            await parser.HandleSubmissionAsync(problemId, submission.Id, request.Content);
            return new SubmitProblemResponse() {
                Success = true,
                SubmissionId = submission.Id,
            };
        }

        public class GetSubmissionsResponse : BaseResponse
        {
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
            public Guid SubmissionId { get; set; }

            /// <summary>
            /// Problem Type specific object describing the submission.
            /// </summary>
            public object Content { get; set; }
        }
        /// <summary>
        /// Gets all submissions in the problemset.
        /// </summary>
        [HttpGet("{Id}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetSubmissions(Guid Id)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(type);
            if(!await manager.IsSubmissionListVisibleAsync(Id))
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetSubmissionsNotViewable",
                };
            }

            var submissions = dbContext.Submissions
                .Where(s => s.ProblemsetId == Id)
                .Select(s => new SubmissionSummaryEntry() {
                    ProblemId = s.ProblemId,
                    ProblemsetProblemId = s.ProblemsetProblem.ProblemsetProblemId,
                    SubmissionId = s.Id,
                    Content = MessagePackSerializer.Deserialize<object>(s.Summary),
                });
            return new GetSubmissionsResponse() {
                Success = true,
                Submissions = submissions,
            };
        }

        /// <summary>
        /// Gets all submissions for a specific problem in the problemset.
        /// </summary>
        [HttpGet("{Id}/problem/{pid}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetProblemSubmissions(Guid Id, string pid)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(type);
            if(!await manager.IsSubmissionListVisibleAsync(Id))
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetSubmissionsNotViewable",
                };
            }
            var problemId = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == Id && psp.ProblemsetProblemId == pid)
                .Select(psp => psp.ProblemId)
                .FirstOrDefaultAsync();
            if(problemId == default(Guid))
            {
                return new GetSubmissionsResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound",
                };
            }
            var submissions = dbContext.Submissions
                .Where(s => s.ProblemsetId == Id && s.ProblemId == problemId)
                .Select(s => new SubmissionSummaryEntry() {
                    ProblemId = s.ProblemId,
                    ProblemsetProblemId = s.ProblemsetProblem.ProblemsetProblemId,
                    SubmissionId = s.Id,
                    Content = MessagePackSerializer.Deserialize<object>(s.Summary),
                });
            return new GetSubmissionsResponse() {
                Success = true,
                Submissions = submissions,
            };
        }

        public class GetSubmissionResponse : BaseResponse
        {
            /// <summary>The global id of the corresponding problem.</summary>
            public Guid ProblemId { get; set; }

            /// <summary>The ID of the corresponding problem.</summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>The ID of the submission.</summary>
            public Guid SubmissionId { get; set; }

            /// <summary>
            /// Problem Type specific object describing the submission.
            /// </summary>
            public object Content { get; set; }
        }
        /// <summary>
        /// Gets a specified submission.
        /// </summary>
        [HttpGet("{Id}/submission/{sid}")]
        public async Task<ActionResult<GetSubmissionResponse>> GetSubmission(Guid Id, Guid sid)
        {
            var type = await dbContext.Problemsets.Where(ps => ps.Id == Id).Select(ps => ps.Type).FirstOrDefaultAsync();
            if(type == null)
            {
                return new GetSubmissionResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            var submission = await dbContext.Submissions
                .Where(s => s.ProblemsetId == Id && s.Id == sid)
                .Select(s => new { Submission = s, ProblemsetProblemId = s.ProblemsetProblem.ProblemsetProblemId })
                .FirstOrDefaultAsync();
            if(submission == null)
            {
                return new GetSubmissionResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetSubmissionNotFound",
                };
            }
            IAsyncProblemsetManager manager = problemsetManagerProvider.GetProblemsetManager(type);
            if(!await manager.IsSubmissionViewableAsync(Id, submission.Submission.Id))
            {
                return new GetSubmissionResponse() {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetSubmissionNotViewable",
                };
            }
            return new GetSubmissionResponse() {
                Success = true,
                ProblemId = submission.Submission.ProblemId,
                ProblemsetProblemId = submission.ProblemsetProblemId,
                SubmissionId = submission.Submission.Id,
                Content = MessagePackSerializer.Deserialize<object>(submission.Submission.Content)
            };
        }
    }
}