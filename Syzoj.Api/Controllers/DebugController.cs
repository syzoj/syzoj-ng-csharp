using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using MessagePack;
using Syzoj.Api.Models.Runner;
using System;
using Syzoj.Api.Services;
using Syzoj.Api.Models;

namespace Syzoj.Api.Controllers
{
    [Route("debug")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private ILegacyRunnerManager runnerManager;

        public DebugController(ILegacyRunnerManager runnerManager)
        {
            this.runnerManager = runnerManager;
        }

        [HttpPost("submit")]
        public async Task<string> SubmitTask([FromBody] BaseApiModel model)
        {
            Console.WriteLine(model._SessionID);
            runnerManager.SubmitTask();
            return "success";
        }
    }
}