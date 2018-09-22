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
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Syzoj.Api.Controllers
{
    [Route("api/problemset")]
    public class ProblemsetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConnectionMultiplexer redis;
        private readonly ProblemParserProvider problemParserProvider;
        private readonly ProblemsetManagerProvider problemsetManagerProvider;
        public ProblemsetController(
            ApplicationDbContext dbContext,
            IConnectionMultiplexer redis,
            ProblemParserProvider problemParserProvider,
            ProblemsetManagerProvider problemsetManagerProvider
        )
        {
            this.dbContext = dbContext;
            this.redis = redis;
            this.problemParserProvider = problemParserProvider;
            this.problemsetManagerProvider = problemsetManagerProvider;
        }

        // NOTE: This is the only Get** function that handles cache lookup within itself
        private async Task<string> GetProblemsetType(Guid problemsetId)
        {
            var type = await redis.GetDatabase().StringGetAsync($"syzoj:problemset:{problemsetId}:type");
            if(type.IsNullOrEmpty)
            {
                var typeString = await dbContext.Problemsets
                    .Where(ps => ps.Id == problemsetId)
                    .Select(ps => ps.Type)
                    .FirstOrDefaultAsync();
                if(typeString == null)
                {
                    return null;
                }
                else
                {
                    await redis.GetDatabase().StringSetAsync($"syzoj:problemset:{problemsetId}:type", typeString);
                    return typeString;
                }
            }
            return type;
        }

        private async Task<string> GetProblemType(Guid pid)
        {
            var type = await dbContext.Problems
                .Where(p => p.Id == pid)
                .Select(p => p.ProblemType)
                .FirstOrDefaultAsync();
            if(type == null)
            {
                return null;
            }
            else
            {
                redis.GetDatabase().StringSet(
                    $"syzoj:problem:{pid}:type",
                    type,
                    flags: CommandFlags.FireAndForget
                );
                return type;
            }
        }

        private async Task<string> GetProblemName(Guid problemsetId, Guid pid)
        {
            var name = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == problemsetId && psp.ProblemId == pid)
                .Select(psp => psp.ProblemsetProblemId)
                .FirstOrDefaultAsync();
            if(name == null)
            {
                return null;
            }
            else
            {
                redis.GetDatabase().HashSet(
                    $"syzoj:problemset:{problemsetId}:problem_name",
                    new HashEntry[] { new HashEntry(pid.ToString(), name) },
                    flags: CommandFlags.FireAndForget
                );
                return name;
            }
        }

        private async Task<Guid> GetProblemId(Guid problemsetId, string name)
        {
            var problemId = await dbContext.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == problemsetId && psp.ProblemsetProblemId == name)
                .Select(psp => psp.ProblemId)
                .FirstOrDefaultAsync();
            if(problemId == null)
            {
                return default(Guid);
            }
            else
            {
                redis.GetDatabase().HashSet(
                    $"syzoj:problemset:{problemsetId}:problem_id",
                    new HashEntry[] { new HashEntry(name, problemId.ToString()) },
                    flags: CommandFlags.FireAndForget
                );
                return problemId;
            }
        }

        private async Task<object> GetProblemSummary(Guid pid, string type)
        {
            var summary = await problemParserProvider.GetParser(type).GetProblemSummaryAsync(pid);
            if(summary == null)
            {
                return null;
            }
            else
            {
                redis.GetDatabase().HashSet(
                    $"syzoj:problem:{pid}:cache",
                    new HashEntry[] { new HashEntry("summary", MessagePackSerializer.Serialize(summary)) },
                    flags: CommandFlags.FireAndForget
                );
                return summary;
            }
        }

        private async Task<object> GetProblemDescription(Guid pid, string type)
        {
            var description = await problemParserProvider.GetParser(type).GetProblemDescriptionAsync(pid);
            if(description == null)
            {
                return null;
            }
            else
            {
                redis.GetDatabase().HashSet(
                    $"syzoj:problem:{pid}:cache",
                    new HashEntry[] { new HashEntry("description", MessagePackSerializer.Serialize(description)) },
                    flags: CommandFlags.FireAndForget
                );
                return description;
            }
        }

        private Guid TakeGuid(string data)
        {
            return Guid.Parse(data.Substring(data.Length - 36));
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
            /// The summary of the problem (usually title).
            /// </summary>
            public object Summary { get; set; }

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
        /// <param name="page">Page number beginning at 0.</param>
        /// <param name="count">Entries per page. At most 50.</param>
        // TODO: Implement sorting and pagination
        [HttpGet("{Id}/problems")]
        public async Task<ActionResult<ProblemsetProblemListResponse>> GetProblemList([FromRoute] Guid Id, [FromQuery] string sort, [FromQuery] string key, [FromQuery] int page, [FromQuery] int count)
        {
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new ProblemsetProblemListResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }
            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            if(!await problemsetPermissionManager.CheckProblemsetPermissionAsync(Id, "view"))
            {
                return new ProblemsetProblemListResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotViewable",
                };
            }

            if(count <= 0 || count > 50)
                count = 50;
            if(page < 0)
                page = 0;
            switch(key)
            {
                case "name":
                    break;
                default:
                    key = "name";
                    break;
            }
            var database = redis.GetDatabase();
            var transaction = database.CreateTransaction();
            var problemIds = await database.SortedSetRangeByRankAsync(
                $"syzoj:problemset:{Id}:sort:{key}",
                page * count,
                page * count + count - 1,
                (sort == "desc" ? Order.Descending : Order.Ascending)
            );
            var tasks = problemIds.Select(pid => {
                var problemId = TakeGuid(pid);
                return new {
                    ProblemId = problemId,
                    ProblemsetProblemId = transaction.HashGetAsync(
                        $"syzoj:problemset:{Id}:problem_name",
                        pid
                    ),
                    Summary = transaction.HashGetAsync(
                        $"syzoj:problem:{problemId}:cache",
                        "summary"
                    ),
                    ProblemType = transaction.StringGetAsync(
                        $"syzoj:problem:{problemId}:type"
                    ),
                };
            }).ToList();
            bool success = await transaction.ExecuteAsync();
            if(!success)
                throw new Exception("Redis transaction failed");
            var result = new List<ProblemsetProblemSummaryEntry>();
            foreach(var r in tasks)
            {
                var entry = new ProblemsetProblemSummaryEntry()
                {
                    ProblemId = r.ProblemId,
                    ProblemsetProblemId = (string) await r.ProblemsetProblemId,
                    Summary = (string) await r.Summary,
                    ProblemType = (string) await r.ProblemType,
                };
                if(entry.ProblemsetProblemId == null)
                    entry.ProblemsetProblemId = await GetProblemName(Id, entry.ProblemId);
                if(entry.ProblemType == null)
                    entry.ProblemType = await GetProblemType(entry.ProblemId);
                if(entry.Summary == null)
                    entry.Summary = await GetProblemSummary(entry.ProblemId, entry.ProblemType);
                result.Add(entry);
            }
            return new ProblemsetProblemListResponse()
            {
                Success = true,
                Problems = result,
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
        [HttpGet("{Id}/problem/{name}")]
        public async Task<ActionResult<GetProblemResponse>> GetProblem(Guid Id, string name)
        {
            // TODO: Cache content in Redis
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new GetProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            var problemIdString = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problemset:{Id}:problem_id",
                name
            );
            Guid problemId;
            if(problemIdString.IsNullOrEmpty)
            {
                problemId = await GetProblemId(Id, name);
            }
            else
            {
                problemId = Guid.Parse((string) problemIdString);
            }

            if(problemId == null)
            {
                return new GetProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound",
                };
            }

            if(!await problemsetPermissionManager.CheckProblemPermissionAsync(Id, problemId, "view"))
            {
                return new GetProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotViewable",
                };
            }

            var problemTypeValue = await redis.GetDatabase().StringGetAsync(
                $"syzoj:problem:{problemId}:type"
            );
            string problemType;
            if(problemTypeValue.IsNullOrEmpty)
            {
                problemType = await GetProblemType(problemId);
            }
            else
            {
                problemType = (string) problemTypeValue;
            }

            var parser = problemParserProvider.GetParser(problemType);
            var description = await parser.GetProblemDescriptionAsync(problemId);
            var submittable = await problemsetPermissionManager.CheckProblemPermissionAsync(Id, problemId, "submit");
            return new GetProblemResponse()
            {
                Success = true,
                ProblemId = problemId,
                ProblemsetProblemId = name,
                Type = problemType,
                Content = description,
                Submittable = submittable,
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
        [HttpPost("{Id}/problem/{name}/submit")]
        public async Task<ActionResult<SubmitProblemResponse>> SubmitProblem([FromRoute] Guid Id, [FromRoute] string name, [FromBody] SubmitProblemRequest request)
        {
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new SubmitProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            var problemIdString = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problemset:{Id}:problem_id",
                name
            );
            Guid problemId;
            if(problemIdString.IsNullOrEmpty)
            {
                problemId = await GetProblemId(Id, name);
            }
            else
            {
                problemId = Guid.Parse((string) problemIdString);
            }

            if(problemId == null)
            {
                return new SubmitProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotFound",
                };
            }

            if(!await problemsetPermissionManager.CheckProblemPermissionAsync(Id, problemId, "submit"))
            {
                return new SubmitProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotSubmittable",
                };
            }

            var problemTypeValue = await redis.GetDatabase().StringGetAsync(
                $"syzoj:problem:{problemId}:type"
            );
            string problemType;
            if(problemTypeValue.IsNullOrEmpty)
            {
                problemType = await GetProblemType(problemId);
            }
            else
            {
                problemType = (string) problemTypeValue;
            }

            var submissionId = Guid.NewGuid();
            var problemsetManager = problemsetManagerProvider.GetProblemsetManager(type);
            await problemsetManager.NewSubmission(Id, problemId, submissionId, request.Content);

            var parser = problemParserProvider.GetParser(problemType);
            await parser.HandleSubmissionAsync(problemId, submissionId);
            return new SubmitProblemResponse()
            {
                SubmissionId = submissionId,
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all submissions for a specific problem in the problemset.
        /// </summary>
        [HttpGet("{Id}/problem/{pid}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetProblemSubmissions(Guid Id, string pid)
        {
            throw new NotImplementedException();
        }

        public class GetSubmissionResponse : BaseResponse
        {
            /// <summary>The global ID of the corresponding problem.</summary>
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
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="sid">The global ID of the submission.</param>
        [HttpGet("{Id}/submission/{sid}")]
        public async Task<ActionResult<GetSubmissionResponse>> GetSubmission(Guid Id, Guid sid)
        {
            throw new NotImplementedException();
        }

        public class CreateProblemRequest
        {
            /// <summary>
            /// The content of the new problem.
            /// </summary>
            [Required]
            public object Content { get; set; }
            /// <summary>
            /// The type of the new problem.
            /// </summary>
            [Required]
            public string ProblemType { get; set; }
        }
        public class CreateProblemResponse : BaseResponse
        {
            /// <summary>
            /// The problem ID of the newly created problem.
            /// </summary>
            public Guid ProblemId { get; set; }
        }

        /// <summary>
        /// Creates a new problem.
        /// </summary>
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="name">The problem identifier in this problemset.</param>
        [HttpPost("{Id}/problem/{name}")]
        public async Task<ActionResult<CreateProblemResponse>> PostProblem([FromRoute] Guid Id, [FromRoute] string name, [FromBody] CreateProblemRequest request)
        {
            if(!ModelState.IsValid)
                return new BadRequestResult();
            // FIXME: This method creates a detached problem if name duplicates.
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new CreateProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            if(!await problemsetPermissionManager.CheckProblemsetPermissionAsync(Id, "edit"))
            {
                return new CreateProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotEditable",
                };
            }
            if(!await problemsetPermissionManager.CheckProblemsetPermissionAsync(Id, "create"))
            {
                return new CreateProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetCreateNotAllowed",
                };
            }

            Console.WriteLine(request.ProblemType);
            var problemParser = problemParserProvider.GetParser(request.ProblemType);
            Console.WriteLine(problemParser);
            if(problemParser == null)
            {
                return new CreateProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.invalidProblemType",
                };
            }

            var problem = new Problem()
            {
                Id = Guid.NewGuid(),
                ProblemType = request.ProblemType,
            };
            dbContext.Problems.Add(problem);
            await dbContext.SaveChangesAsync();

            var problemsetManager = problemsetManagerProvider.GetProblemsetManager(type);
            await problemsetManager.AttachProblem(Id, problem.Id, name);
            await problemsetManager.PutProblem(Id, problem.Id, request.Content);
            return new CreateProblemResponse()
            {
                Success = true,
                ProblemId = problem.Id,
            };
        }

        public class PutProblemRequest
        {
            /// <summary>
            /// The content of the new problem.
            /// </summary>
            public object Content { get; set; }
        }
        public class PutProblemResponse : BaseResponse
        {

        }
        /// <summary>
        /// Overwrites the problem.
        /// </summary>
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="name">The problem identifier in this problemset.</param>
        [HttpPut("{Id}/problem/{name}")]
        public async Task<ActionResult<PutProblemResponse>> PutProblem([FromRoute] Guid Id, [FromRoute] string name, [FromBody] PutProblemRequest request)
        {
            // FIXME: This method creates a detached problem if name duplicates.
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new PutProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            var problemIdString = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problemset:{Id}:problem_id",
                name
            );
            Guid problemId;
            if(problemIdString.IsNullOrEmpty)
            {
                problemId = await GetProblemId(Id, name);
            }
            else
            {
                problemId = Guid.Parse((string) problemIdString);
            }

            if(!await problemsetPermissionManager.CheckProblemPermissionAsync(Id, problemId, "edit"))
            {
                return new PutProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotEditable",
                };
            }

            var problemsetManager = problemsetManagerProvider.GetProblemsetManager(type);
            await problemsetManager.PutProblem(Id, problemId, request.Content);
            return new PutProblemResponse()
            {
                Success = true,
            };
        }

        public class PatchProblemRequest
        {
            /// <summary>
            /// The content of the new problem.
            /// </summary>
            public object Content { get; set; }
        }
        public class PatchProblemResponse : BaseResponse
        {

        }
        /// <summary>
        /// Patches part of the problem.
        /// </summary>
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="name">The problem identifier in this problemset.</param>
        [HttpPatch("{Id}/problem/{name}")]
        public async Task<ActionResult<PatchProblemResponse>> PatchProblem([FromRoute] Guid Id, [FromRoute] string name, [FromBody] PutProblemRequest request)
        {
            // FIXME: This method creates a detached problem if name duplicates.
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new PatchProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            var problemIdString = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problemset:{Id}:problem_id",
                name
            );
            Guid problemId;
            if(problemIdString.IsNullOrEmpty)
            {
                problemId = await GetProblemId(Id, name);
            }
            else
            {
                problemId = Guid.Parse((string) problemIdString);
            }

            if(!await problemsetPermissionManager.CheckProblemPermissionAsync(Id, problemId, "edit"))
            {
                return new PatchProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotEditable",
                };
            }

            var problemsetManager = problemsetManagerProvider.GetProblemsetManager(type);
            await problemsetManager.PatchProblem(Id, problemId, request.Content);
            return new PatchProblemResponse()
            {
                Success = true,
            };
        }

        public class ExportProblemResponse : BaseResponse
        {
            public object Content { get; set; }
        }
        /// <summary>
        /// Exports the problem.
        /// </summary>
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="name">The problem identifier in this problemset.</param>
        [HttpGet("{Id}/problem/{name}/export")]
        public async Task<ActionResult<ExportProblemResponse>> ExportProblem([FromRoute] Guid Id, [FromRoute] string name)
        {
            // FIXME: This method creates a detached problem if name duplicates.
            var type = await GetProblemsetType(Id);
            if(type == null)
            {
                return new ExportProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetNotFound",
                };
            }

            var problemsetPermissionManager = problemsetManagerProvider.GetProblemsetPermissionManager(type);
            var problemIdString = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problemset:{Id}:problem_id",
                name
            );
            Guid problemId;
            if(problemIdString.IsNullOrEmpty)
            {
                problemId = await GetProblemId(Id, name);
            }
            else
            {
                problemId = Guid.Parse((string) problemIdString);
            }

            if(!await problemsetPermissionManager.CheckProblemPermissionAsync(Id, problemId, "export"))
            {
                return new ExportProblemResponse()
                {
                    Success = false,
                    ErrorMessage = "syzoj.error.problemsetProblemNotExportable",
                };
            }

            var problem = await dbContext.Problems.FindAsync(problemId);
            return new ExportProblemResponse()
            {
                Success = true,
                Content = JsonConvert.DeserializeObject(problem.Content),
            };
        }
    }
}