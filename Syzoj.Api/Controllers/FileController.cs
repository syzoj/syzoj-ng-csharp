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
                    return StatusCode(403);
                var realPath = Path.Combine(provider.GetPath(), path);
                if(System.IO.File.Exists(realPath))
                    return PhysicalFile(realPath, "application/octet-stream", fileName, true);
                else
                    return NotFound();
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
                    return StatusCode(403);
                var realPath = Path.Combine(provider.GetPath(), path);
                var dir = Path.GetDirectoryName(realPath);
                System.IO.Directory.CreateDirectory(dir);
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