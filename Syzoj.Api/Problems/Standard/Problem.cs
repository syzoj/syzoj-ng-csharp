using System;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class Problem : DbModelObjectBase<Model.Problem>, IProblem, IProblemContract
    {
        private ProblemStatement statement;
        public Problem(IServiceProvider serviceProvider, ApplicationDbContext dbContext, Model.Problem model) : base(serviceProvider, dbContext, model)
        {
            this.statement = MessagePackSerializer.Deserialize<ProblemStatement>(model._Statement);
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

        public Task<IProblemSubmission> ClaimSubmission(string token)
        {

            throw new NotImplementedException();
        }
    }
}