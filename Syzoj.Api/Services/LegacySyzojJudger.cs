using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Syzoj.Api.Services
{
    public class LegacySyzojJudger : ILegacySyzojJudger
    {
        private readonly IModel sess;
        public LegacySyzojJudger(IConnection conn)
        {
            this.sess = conn.CreateModel();
            this.sess.QueueDeclare("judge", durable: true, arguments: new Dictionary<string, object>() {
                { "maxPriority", 5 },
            });
        }

        public Task SendJudgeRequestAsync(int priority, byte[] data)
        {
            this.sess.BasicPublish(exchange: "", routingKey: "judge", body: data);
            return Task.CompletedTask;
        }
    }
}