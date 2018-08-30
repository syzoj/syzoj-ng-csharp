using System;
using System.Collections.Generic;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Syzoj.Api.Models.Runner;

namespace Syzoj.Api.Services
{
    public class LegacyRunnerManager : ILegacyRunnerManager
    {
        private IConnection rmqConnection;
        private IModel channel;

        public LegacyRunnerManager(IConnection rmqConnection)
        {
            this.rmqConnection = rmqConnection;
            this.channel = rmqConnection.CreateModel();
            channel.QueueDeclareNoWait("judge", true, false, false, new Dictionary<string, object>() {
                {"x-max-priority", 5}
            });
            channel.QueueDeclareNoWait("result", true, false, false, null);
            channel.ExchangeDeclareNoWait("progress", "fanout", false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) => {
                try {
                    var body = ea.Body;
                    Console.WriteLine(MessagePackSerializer.ToJson(body));
                    var data = MessagePackSerializer.Deserialize<LegacyProgressReportData>(body);
                    Console.WriteLine(MessagePackSerializer.ToJson(data));
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            };
            channel.BasicConsume(queue: "result", autoAck: false, consumer: consumer);
        }
        public void SubmitTask(LegacyJudgeRequest req)
        {
            byte[] body = MessagePackSerializer.Serialize(req);
            channel.BasicPublish(
                exchange: "",
                routingKey: "judge",
                mandatory: true,
                basicProperties: null,
                body: body
            );
        }
    }
}