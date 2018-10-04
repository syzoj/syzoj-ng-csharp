using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardJudgeHub : Hub<IStandardJudgeClient>
    {
        private readonly IConnection amqpConnection;
        private readonly IHubContext<StandardJudgeHub, IStandardJudgeClient> hubContext;
        private readonly ILogger<StandardJudgeHub> logger;
        private readonly IConnectionMultiplexer redis;

        public StandardJudgeHub(IConnection amqpConnection, IHubContext<StandardJudgeHub, IStandardJudgeClient> hubContext, ILogger<StandardJudgeHub> logger, IConnectionMultiplexer redis)
        {
            this.amqpConnection = amqpConnection;
            this.hubContext = hubContext;
            this.logger = logger;
            this.redis = redis;
        }
        public Task ConsumeStandardQueue()
        {
            var data = (HubData) Context.Items["data"];
            var consumer = new AsyncEventingBasicConsumer(data.Model);
            consumer.Received += OnReceivedTask;
            data.Model.BasicConsume("standard-judge", false, consumer);
            logger.LogDebug("ConsumeStandardQueue");
            return Task.CompletedTask;
        }

        private async Task OnReceivedTask(object sender, BasicDeliverEventArgs args)
        {
            var data = (HubData) Context.Items["data"];
            var submissionId = new Guid(args.Body);
            logger.LogInformation("Received Task {Id}", submissionId);

            var result = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problem-standard:{submissionId}:data",
                new RedisValue[] { "problemId", "code", "language", "data" }
            );
            var problemId = new Guid((string) result[0]);
            var code = (string) result[1];
            var language = (string) result[2];
            var content = MessagePackSerializer.Deserialize<StandardProblemContent>(result[3]);

            var status = new SubmissionStatus() {
                DeliveryTag = args.DeliveryTag,
                SubmissionId = submissionId,
                Language = language,
                Code = code,
                Content = content,
            };
            var id = Guid.NewGuid();
            data.Submissions.Add(id, status);
            await Clients.Caller.HandleSubmission(id.ToString(), status.Content);
        }

        public override Task OnConnectedAsync()
        {
            var data = new HubData();
            Context.Items["data"] = data;
            data.Model = amqpConnection.CreateModel();
            data.Model.BasicQos(0, 2, true);
            data.Submissions = new Dictionary<Guid, SubmissionStatus>();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var data = (HubData) Context.Items["data"];
            data.Model.Close();
            return base.OnDisconnectedAsync(exception);
        }

        private class HubData
        {
            public IModel Model { get; set; }
            public IDictionary<Guid, SubmissionStatus> Submissions { get; set; }
        }
        private class SubmissionStatus
        {
            public ulong DeliveryTag { get; set; }
            public Guid SubmissionId { get; set; }
            public string Language { get; set; }
            public string Code { get; set; }
            public StandardProblemContent Content { get; set; }
        }
    }
}