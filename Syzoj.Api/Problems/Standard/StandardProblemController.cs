using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<CustomResponse<Guid>>> Create()
        {
            var problemResolver = await resolverService.CreateNewProblem("standard");
            return new CustomResponse<Guid>(problemResolver.Id);
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
    }
}