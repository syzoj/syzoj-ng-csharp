using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Services;
using System.IO;
using System;

namespace Syzoj.Api.Controllers
{
    [Route("file")]
    public class FileController : ControllerBase
    {
        private readonly IAsyncFileStorageProvider provider;
        public FileController(IAsyncFileStorageProvider provider)
        {
            this.provider = provider;
        }

        [HttpGet("download")]
        public IActionResult Download([FromQuery] long expire, [FromQuery] string fileName, [FromQuery] string path, [FromQuery] string signature)
        {
            if(this.provider is LocalFileStorageProvider provider)
            {
                var str = "download" + "\n" + path + "\n" + expire;
                if(!provider.VerifySignature(str, signature))
                    return BadRequest();
                if(expire < ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds())
                    return Forbid("Expired");
                var realPath = Path.Join(provider.GetPath(), path);
                return PhysicalFile(realPath, "application/octet-stream", fileName, true);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("upload")]
        public async Task<IActionResult> Upload([FromQuery] long expire, [FromQuery] string path, [FromQuery] string signature)
        {
            if(this.provider is LocalFileStorageProvider provider)
            {
                var str = "upload" + "\n" + path + "\n" + expire;
                if(!provider.VerifySignature(str, signature))
                    return BadRequest();
                if(expire < ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds())
                    return Forbid("Expired");
                var realPath = Path.Join(provider.GetPath(), path);
                using(var fileStream = System.IO.File.Create(realPath))
                {
                    await HttpContext.Request.Body.CopyToAsync(fileStream);
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}