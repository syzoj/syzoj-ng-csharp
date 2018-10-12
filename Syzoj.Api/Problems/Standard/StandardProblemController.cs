using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Mvc;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    [Route("api/problem/standard/{problemId}")]
    [ApiController]
    public class StandardProblemController : CustomControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IProblemResolverService resolverService;

        public StandardProblemController(ApplicationDbContext dbContext, IProblemResolverService resolverService)
        {
            this.dbContext = dbContext;
            this.resolverService = resolverService;
        }

        [HttpGet("view")]
        public async Task<ActionResult<CustomResponse<StandardProblemContent>>> View(Guid problemId)
        {
            var problem = await dbContext.Set<Problem>().FindAsync(problemId);
            if(problem == null)
                return NotFound();
            var problemContent = MessagePackSerializer.Deserialize<StandardProblemContent>(problem.Data);
            return new CustomResponse<StandardProblemContent>(problemContent);
        }

        [HttpPost("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> Create([FromBody] StandardProblemContent initialContent)
        {
            var problemId = Guid.NewGuid();
            await resolverService.RegisterProblem(problemId, "standard");
            // TODO: Potential data corruption
            var problem = await dbContext.Set<Problem>().FindAsync(problemId);
            problem.Data = MessagePackSerializer.Serialize(initialContent);
            await dbContext.SaveChangesAsync();
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

        [HttpGet("download/{*fileName:required}")]
        public async Task<ActionResult<CustomResponse<string>>> Download(Guid problemId, string fileName)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                var url = await standardProblemResolver.GenerateDownloadLink(fileName);
                return new CustomResponse<string>(url);
            }
            else
            {
                return BadRequest();
            }
        }

        public class UploadResponse
        {
            public string UploadLink { get; set; }
            public string FileName { get; set; }
        }
        [HttpPost("upload/{*fileName:required}")]
        public async Task<ActionResult<CustomResponse<UploadResponse>>> Upload(Guid problemId, string fileName)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                var url = await standardProblemResolver.GenerateUploadLink(fileName);
                return new CustomResponse<UploadResponse>(new UploadResponse() {
                    UploadLink = url,
                    FileName = fileName,
                });
            }
            else
            {
                return BadRequest();
            }
        }

        public class StandardProblemSubmissionRequest
        {
            public string Language { get; set; }
            public string Code { get; set; }
        }
        [HttpPost("submission/{submissionId}/submit")]
        public async Task<ActionResult<CustomResponse<bool>>> Submit(Guid problemId, Guid submissionId, [FromBody] StandardProblemSubmissionRequest request)
        {
            var problemResolver = await resolverService.GetProblemResolver(problemId);
            if(problemResolver is StandardProblemResolver standardProblemResolver)
            {
                return new CustomResponse<bool>(await standardProblemResolver.SubmitCodeAsync(submissionId, request.Language, request.Code));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}