using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Syzoj.Api.Data;
using Syzoj.Api.Mvc;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problems.Standard
{
    [Route("api/problem-standard")]
    [ApiController]
    public class ProblemController : CustomControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<CustomResponse<Guid>>> CreateProblem(
            [FromServices] IObjectService objectService,
            [FromServices] ProblemProvider problemProvider,
            [FromServices] ApplicationDbContext dbContext,
            [FromBody] Model.ProblemStatement statement
        )
        {
            var problem = await problemProvider.CreateObject(dbContext, statement);
            return new CustomResponse<Guid>(problem.Id);
        }

        public class SubmitRequest
        {
            public string Language { get; set; }
            public string Code { get; set; }
        }
        public class SubmitResponse
        {
            /// <summary>
            /// A one-pass token passed to problemset to notify a new submission.
            /// </summary>
            public string Token { get; set; }
        }
        [HttpPost("{problemId}/submit")]
        public async Task<ActionResult<CustomResponse<SubmitResponse>>> Submit(
            [FromServices] IConnectionMultiplexer redis,
            [FromServices] ILogger<ProblemController> logger,
            [FromBody] SubmitRequest request
        )
        {
            var token = Utils.GenerateToken(32);
            logger.LogDebug(Newtonsoft.Json.JsonConvert.SerializeObject(request));

            var database = redis.GetDatabase();
            var key = $"problem:standard:submission:{token}";
            await Task.WhenAll(
                database.HashSetAsync(
                    key,
                    new HashEntry[] {
                        new HashEntry("lang", request.Language),
                        new HashEntry("code", request.Code)
                    }
                ),
                database.KeyExpireAsync(
                    key,
                    TimeSpan.FromMinutes(10)
                )
            );

            return new CustomResponse<SubmitResponse>(new SubmitResponse() {
                Token = token
            });
        }
    }
}