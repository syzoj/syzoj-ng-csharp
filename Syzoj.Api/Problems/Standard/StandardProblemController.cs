using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Mvc;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    [Route("api/problem/{problemId}/standard")]
    [ApiController]
    public class StandardProblemController : CustomControllerBase
    {
        private readonly IProblemResolverService resolverService;

        public StandardProblemController(IProblemResolverService resolverService)
        {
            this.resolverService = resolverService;
        }

        [HttpGet("view")]
        public async Task<ActionResult<CustomResponse<StandardProblemContent>>> View(Guid problemId)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                return new CustomResponse<StandardProblemContent>(await standardProblemResolver.GetContent());
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> Create([FromServices] ApplicationDbContext context)
        {
            var problemId = Guid.NewGuid();
            var problem = new Problem() {
                Id = problemId,
                ProblemType = "standard",
            };
            context.Problems.Add(problem);
            var problemData = new StandardProblemContent();
            problem.Data = MessagePackSerializer.Serialize(problemData);
            await context.SaveChangesAsync();;
            return new CustomResponse<Guid>(problemId);
        }

        [HttpPut("put")]
        public async Task<ActionResult<CustomResponse<bool>>> Put(Guid problemId, [FromServices] ApplicationDbContext context, [FromBody] StandardProblemContent content)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                return new CustomResponse<bool>(await standardProblemResolver.Put(content));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("download/{fileId}/{fileName}")]
        public async Task<ActionResult<CustomResponse<string>>> Download(Guid problemId, Guid fileId, string fileName)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                return new CustomResponse<string>(await standardProblemResolver.GenerateDownloadLink(fileId, fileName));
            }
            else
            {
                return BadRequest();
            }
        }

        public class UploadResponse
        {
            public string UploadLink { get; set; }
            public Guid FileId { get; set; }
        }
        [HttpPost("upload")]
        public async Task<ActionResult<CustomResponse<UploadResponse>>> Upload(Guid problemId)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                Guid fileId = Guid.NewGuid();
                return new CustomResponse<UploadResponse>(new UploadResponse() {
                    UploadLink = await standardProblemResolver.GenerateUploadLink(fileId),
                    FileId = fileId,
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("submission/{submissionId}/submit")]
        public async Task<ActionResult<CustomResponse<bool>>> Submit(Guid problemId, Guid submissionId)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                return new CustomResponse<bool>(await standardProblemResolver.SubmitCodeAsync(submissionId, "language", "code"));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}