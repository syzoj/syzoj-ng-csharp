using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemJudger
    {
        private readonly IConnection conn;
        private readonly IModel model;

        public StandardProblemJudger(IConnection conn)
        {
            this.conn = conn;
            this.model = conn.CreateModel();
            
            this.model.QueueDeclare("standard-judge", true, false, false, new Dictionary<string, object>() {
                { "maxPriority", 5 },
            });
            this.model.ExchangeDeclare("standard-progress", "fanout", false, false);
            this.model.QueueDeclare("standard-result", true, false, false);

            var listener = new AsyncEventingBasicConsumer(model);
            listener.Received += Received;
            this.model.BasicConsume("standard-judge", false, listener);
        }

        private Task Received(object sender, BasicDeliverEventArgs args)
        {
            Guid submissionId = new Guid(args.Body);
            System.Console.WriteLine($"Process submission {submissionId}");
            return Task.FromException(new NotImplementedException()); // is not logged nor nacked
        }

        public Task SubmitJudge(Guid submissionId)
        {
            this.model.BasicPublish("", "standard-judge", body: submissionId.ToByteArray());
            return Task.CompletedTask;
        }
    }
}