using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using Syzoj.Api.Data;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    // According to source code at https://github.com/aspnet/SignalR/blob/release/2.2/src/Microsoft.AspNetCore.SignalR.Core/HubConnectionHandler.cs
    // It seems that all calls are serialized so we're not handling concurrency here
    public class JudgerHub : Hub<IJudger>
    {
        private readonly IServiceProvider serviceProvider;

        public JudgerHub(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Connect(string _id, string token)
        {
            Guid id;
            if(!Guid.TryParse(_id, out id))
                return Task.FromResult(false);

            var data = (HubData) base.Context.Items["data"];
            return data.Connect(id, token);
        }

        public override async Task OnConnectedAsync()
        {
            var data = new HubData(serviceProvider.CreateScope().ServiceProvider, base.Context.ConnectionId);
            base.Context.Items["data"] = data;
            await data.OnConnectedAsync();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var data = (HubData) base.Context.Items["data"];
            await data.OnDisconnectedAsync();
            await base.OnDisconnectedAsync(exception);
        }

        private class HubData
        {
            private readonly IConnection rmqConnection;
            private readonly IConnectionMultiplexer redis;
            private readonly ApplicationDbContext dbContext;
            private readonly IServiceProvider serviceProvider;
            private readonly IHubContext<JudgerHub, IJudger> hubContext;
            private readonly ILogger<JudgerHub> logger;
            private readonly string connectionId;

            private bool isLoggedIn;
            private IModel rmqModel;
            private Guid judgerId;
            private IDictionary<Guid, JudgeSession> sessions = new Dictionary<Guid, JudgeSession>();

            public HubData(IServiceProvider serviceProvider, string connectionId)
            {
                this.serviceProvider = serviceProvider;
                this.connectionId = connectionId;

                this.rmqConnection = serviceProvider.GetRequiredService<IConnection>();
                this.redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
                this.dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                this.hubContext = serviceProvider.GetRequiredService<IHubContext<JudgerHub, IJudger>>();
                this.logger = serviceProvider.GetRequiredService<ILogger<JudgerHub>>();
            }

            public async Task<bool> Connect(Guid id, string token)
            {
                if(isLoggedIn)
                {
                    logger.LogWarning("{connectionId}: Rejecting connection request because it is already logged in as {judgerId}", connectionId, judgerId);
                    return false;
                }

                var model = await dbContext.FindAsync<Model.Judger>(id);
                if(model == null || model.Token != token)
                {
                    logger.LogWarning("{connectionId}: Rejecting connection request because judger id or token is incorrect", connectionId);
                    return false;
                }

                var key = $"problem:standard:judger:{id}";
                var result = await redis.GetDatabase().StringSetAsync(key, RedisValue.EmptyString, TimeSpan.FromMinutes(5), When.NotExists);
                if(!result)
                {
                    logger.LogWarning("{connectionId}: Rejecting connection request because {judgerId} is already logged in elsewhere", connectionId, id);
                    return false;
                }
                judgerId = id;
                isLoggedIn = true;

                rmqModel = rmqConnection.CreateModel();
                rmqModel.QueueDeclare("standard-judge", true, false, false, new Dictionary<string, object>() {
                    { "maxPriority", 5 },
                });
                rmqModel.BasicQos(0, 3, false);
                rmqModel.BasicConsume(queue: "standard-judge", consumer: new QueueConsumer(this), autoAck: false);
                logger.LogDebug("Judger {Id} connected successfully", id);
                return true;
            }

            public IJudger GetClient()
            {
                return hubContext.Clients.Client(connectionId);
            }

            public async Task OnDisconnectedAsync()
            {
                if(isLoggedIn)
                {
                    foreach(var kv in sessions)
                    {
                        await kv.Value.Abort();
                    }
                    rmqModel.Close();
                    var key = $"problem:standard:judger:{judgerId}";
                    await redis.GetDatabase().KeyDeleteAsync(key);
                }
            }

            public Task OnConnectedAsync()
            {
                return Task.CompletedTask;
            }

            private class QueueConsumer : AsyncDefaultBasicConsumer
            {
                private readonly HubData hubData;

                public QueueConsumer(HubData hubData)
                {
                    this.hubData = hubData;
                }
                public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
                {
                    await base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
                    
                    var submissionId = new Guid(body);
                    hubData.logger.LogDebug("Judger {judgerId} received task {submissionId}", hubData.judgerId, submissionId);
                    var database = hubData.redis.GetDatabase();
                    var status = await database.StringGetAsync($"problem:standard:submission-status:{submissionId}:status");
                    if(status == 2)
                    {
                        await database.KeyDeleteAsync(new RedisKey[] {
                            $"problem:standard:submission-status:{submissionId}:status",
                            $"problem:standard:submission-status:{submissionId}:data"
                        });
                        hubData.logger.LogInformation("Skipping aborted submission {submissionId}", submissionId);
                        hubData.rmqModel.BasicNack(deliveryTag, false, false);
                        return;
                    }
                    else if(status.IsNull)
                    {
                        hubData.logger.LogWarning("Skipping submission without status {submissionId}", submissionId);
                        hubData.rmqModel.BasicNack(deliveryTag, false, false);
                    }
                    
                    var sessionId = Guid.NewGuid();
                    var session = new JudgeSession(hubData, sessionId, submissionId, deliveryTag);
                    hubData.sessions.Add(sessionId, session);
                    try
                    {
                        await session.Start();
                    }
                    catch(Exception e)
                    {
                        hubData.logger.LogError(e, "Failed to start judge session");
                        hubData.rmqModel.BasicNack(deliveryTag, false, true);
                        throw;
                    }
                }
            }

            private class JudgeSession
            {
                private readonly HubData hubData;
                private readonly Guid submissionId;
                private readonly Guid sessionId;
                private readonly ulong deliveryTag;
                private string Language;
                private string Code;
                private StandardProblemContent Content;
                private ILogger<JudgeSession> logger;
                public JudgeSession(HubData hubData, Guid sessionId, Guid submissionId, ulong deliveryTag)
                {
                    this.hubData = hubData;
                    this.logger = hubData.serviceProvider.GetRequiredService<ILogger<JudgeSession>>();
                    this.submissionId = submissionId;
                    this.sessionId = sessionId;
                    this.deliveryTag = deliveryTag;
                }

                public async Task Start()
                {
                    var database = hubData.redis.GetDatabase();
                    await database.StringSetAsync($"problem:standard:submission-status:{submissionId}:status", 1);

                    var submissionModel = await hubData.dbContext.FindAsync<Model.ProblemSubmission>(submissionId);
                    if(submissionModel == null)
                    {
                        logger.LogWarning("Submission {submissionId} does not have corresponding database entry", submissionId);
                        hubData.rmqModel.BasicNack(deliveryTag, false, false);
                        return;
                    }

                    var problemModel = await hubData.dbContext.FindAsync<Model.Problem>(submissionModel.ProblemId);
                    var _testData = problemModel._TestData;
                    if(_testData == null)
                    {
                        logger.LogWarning("Problem {problemId} does not have testdata", problemModel.Id);
                        hubData.rmqModel.BasicNack(deliveryTag, false, false);
                        return;
                    }

                    var testData = MessagePackSerializer.Deserialize<Model.StandardTestData>(_testData);
                    await hubData.GetClient().OnSubmission(sessionId, testData);
                }

                public Task Abort()
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}