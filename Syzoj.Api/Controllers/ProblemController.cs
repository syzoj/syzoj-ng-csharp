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
    public class ProblemController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILegacySyzojImporter legacySyzojImporter;
        private readonly ILegacyRunnerManager legacyRunnerManager;
        public ProblemController(ApplicationDbContext dbContext, ILegacySyzojImporter legacySyzojImporter, ILegacyRunnerManager legacyRunnerManager)
        {
            this.dbContext = dbContext;
            this.legacySyzojImporter = legacySyzojImporter;
            this.legacyRunnerManager = legacyRunnerManager;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProblem(string id)
        {
            var problem = await dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1 && psp.ProblemSetProblemId == id)
                .Include(psp => psp.Problem)
                .Select(p => new {
                    Id = p.ProblemSetProblemId,
                    Title = p.Problem.Title,
                    Submissions = p.Submissions,
                    Accepts = p.Accepts,
                    DataType = p.Problem.Type,
                    Statement = p.Problem.Statement,
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

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitProblem(string id, [FromBody]ProblemCodeSubmissionRequest req)
        {
            var problem = await dbContext.ProblemSetProblems
                .Where(psp => psp.ProblemSetId == 1 && psp.ProblemSetProblemId == id)
                .Select(psp => psp.Problem)
                .SingleOrDefaultAsync();
            if(problem == null)
            {
                return NotFound(new {
                    Status = "Fail",
                    Message = "Problem not found"
                });
            }
            switch(problem.Type)
            {
                case ProblemType.SyzojLegacyTraditional:
                    var traditionalProblemData = MessagePackSerializer.Deserialize<SyzojLegacyTraditionalProblemData>(problem._Data);
                    legacyRunnerManager.SubmitTask(new LegacyJudgeRequest() {
                        content = new LegacyJudgeRequest.Content {
                            taskId = Newtonsoft.Json.JsonConvert.SerializeObject(new {
                                ProblemSetId = 1,
                                ProblemId = id,
                            }),
                            testData = problem.Id.ToString(),
                            type = LegacyProblemType.Standard,
                            priority = 2,
                            param = new {
                                language = req.Language,
                                code = req.Code,
                                timeLimit = traditionalProblemData.TimeLimit,
                                memoryLimit = traditionalProblemData.MemoryLimit,
                                fileIOInput = (traditionalProblemData.FileIo ? traditionalProblemData.FileIoInputName : null),
                                fileIOOutput = (traditionalProblemData.FileIo ? traditionalProblemData.FileIoOutputName : null),
                            },
                        },
                        extraData = null,
                    });
                    return Ok(new {
                        Status = "Success",
                    });
                case ProblemType.SyzojLegacySubmitAnswer:
                    legacyRunnerManager.SubmitTask(new LegacyJudgeRequest() {
                        content = new LegacyJudgeRequest.Content {
                            taskId = Newtonsoft.Json.JsonConvert.SerializeObject(new {
                                ProblemSetId = 1,
                                ProblemId = id,
                            }),
                            testData = problem.Id.ToString(),
                            type = LegacyProblemType.AnswerSubmission,
                            priority = 2,
                            param = null,
                        },
                        extraData = "submit-answer-zip-file",
                    });
                    return Ok(new {
                        Status = "Success",
                    });
                case ProblemType.SyzojLegacyInteraction:
                    var interactionProblemData = MessagePackSerializer.Deserialize<SyzojLegacyTraditionalProblemData>(problem._Data);
                    legacyRunnerManager.SubmitTask(new LegacyJudgeRequest() {
                        content = new LegacyJudgeRequest.Content {
                            taskId = Newtonsoft.Json.JsonConvert.SerializeObject(new {
                                ProblemSetId = 1,
                                ProblemId = id,
                            }),
                            testData = problem.Id.ToString(),
                            type = LegacyProblemType.Standard,
                            priority = 2,
                            param = new {
                                language = req.Language,
                                code = req.Code,
                                timeLimit = interactionProblemData.TimeLimit,
                                memoryLimit = interactionProblemData.MemoryLimit,
                            },
                        },
                        extraData = null,
                    });
                    return Ok(new {
                        Status = "Success",
                    });
                default:
                    return Ok(new {
                        Status = "Fail",
                        Message = "Unknown problem type",
                    });
            }
        }

        [HttpPost("{id}/import/legacy-syzoj")]
        public async Task<IActionResult> ImportProblemFromLegacySyzoj(string id, [FromBody] ImportProblemFromLegacySyzojRequest req)
        {
            var problem = await legacySyzojImporter.ImportFromLegacySyzojAsync(req.ProblemURL);
            if(problem == null)
            {
                return Ok(new {
                    Status = "Fail",
                    Message = "Import failed"
                });
            }
            dbContext.Problems.Add(problem);
            var problemSetRelation = new ProblemSetProblem() {
                Problem = problem,
                ProblemSetId = 1,
                ProblemSetProblemId = id,
                Submissions = 0,
                Accepts = 0,
            };
            dbContext.ProblemSetProblems.Add(problemSetRelation);
            // TODO: Handle uniqueness violation
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException ne && ne.SqlState.Equals("23505"))
                {
                    switch(ne.ConstraintName)
                    {
                        case "PK_ProblemSetProblems":
                            return Conflict(new {
                                Status = "Fail",
                                Message = "Problem with the same ID already exists",
                            });
                        case "IX_ProblemSetProblems_ProblemSetId_ProblemId":
                            return Conflict(new {
                                Status = "Fail",
                                Message = "The same problem already exists in the problemset"
                            });
                    }
                }
            }
            return Ok(new {
                Status = "Success",
            });
        }
    }
}