using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problems.Standard
{
    public class JudgeServer
    {
        private readonly IConnection rmqConn;
        private readonly IConnectionMultiplexer redis;
        private readonly ILogger<JudgeServer> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private IModel rmqModel;

        public JudgeServer(IConnection rmqConn, IConnectionMultiplexer redis, ILogger<JudgeServer> logger, IServiceScopeFactory scopeFactory)
        {
            this.rmqConn = rmqConn;
            this.redis = redis;
            this.logger = logger;
            this.scopeFactory = scopeFactory;
            this.rmqModel = rmqConn.CreateModel();
        }

        public async Task SubmitJudge(Guid submissionId)
        {
            var database = redis.GetDatabase();
            var status = await database.StringGetAsync($"problem:standard:submission-status:{submissionId}:status");
            if(!status.IsNull)
            {
                if(status == 2)
                {
                    // Judge an aborted submission
                    await database.StringSetAsync($"problem:standard:submission-status:{submissionId}:status", 0);
                }
                else
                {
                    logger.LogWarning("Ignored attempt to judge a running task {submissionId}", submissionId);
                }
                return;
            }

            await database.StringSetAsync($"problem:standard:submission-status:{submissionId}:status", 0);
            this.rmqModel.BasicPublish("", "standard-judge", body: submissionId.ToByteArray());
        }
    }
}