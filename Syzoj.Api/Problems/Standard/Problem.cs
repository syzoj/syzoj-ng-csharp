using System;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class Problem : DbModelObjectBase<Model.Problem>, IProblem, IProblemContract
    {
        private ProblemStatement statement;
        private ProblemSubmissionProvider submissionProvider;
        public Problem(IServiceProvider serviceProvider, ApplicationDbContext dbContext, Model.Problem model) : base(serviceProvider, dbContext, model)
        {
            this.statement = MessagePackSerializer.Deserialize<ProblemStatement>(model._Statement);
            this.submissionProvider = ServiceProvider.GetRequiredService<ProblemSubmissionProvider>();
        }

        Task IProblemContract.CancelContract()
        {
            return Task.CompletedTask;
        }

        Task<IProblemContract> IProblem.CreateProblemContract()
        {
            return Task.FromResult<IProblemContract>(this);
        }

        public Task<ViewModel> GetProblemContent()
        {
            return Task.FromResult(new ViewModel() {
                Template = "problem-standard-view",
                Content = new {
                    Statement = statement
                }
            });
        }

        public Task RequestUpdateNotification(IProblemUpdateCallback callback)
        {
            throw new NotImplementedException();
        }

        public async Task<IProblemSubmission> ClaimSubmission(string token)
        {
            var redis = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

            var transaction = redis.GetDatabase().CreateTransaction();
            var key = $"problem:standard:submission:{token}";
            var entriesTask = transaction.HashGetAllAsync(key);
            transaction.KeyDeleteAsync(key);
            await transaction.ExecuteAsync();
            var entries = entriesTask.Result;

            if(!entries.Any())
            {
                return null;
            }

            var dic = entries.ToDictionary<HashEntry, string, string>(h => h.Name, h => h.Value);
            return await submissionProvider.CreateObject(DbContext, Model.Id, dic["lang"], dic["code"]);
        }
    }
}