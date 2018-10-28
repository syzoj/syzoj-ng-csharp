using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class ProblemSubmission : DbModelObjectBase<Model.ProblemSubmission>, IProblemSubmission
    {
        private readonly ILogger<ProblemSubmission> logger;

        public ProblemSubmission(IServiceProvider serviceProvider, ApplicationDbContext dbContext, Model.ProblemSubmission model) : base(serviceProvider, dbContext, model)
        {
            this.logger = serviceProvider.GetRequiredService<ILogger<ProblemSubmission>>();
        }

        public async Task<IProblem> GetProblem()
        {
            var problemModel = await DbContext.FindAsync<Model.Problem>(Model.ProblemId);
            return new Problem(ServiceProvider, DbContext, problemModel);
        }

        public Task<ViewModel> GetSubmissionContent()
        {
            return Task.FromResult(new ViewModel() {
                Template = "problem-standard-submission",
                Content = new {
                    SubmissionId = Model.Id,
                    ViewToken = Model.ViewToken,
                    Code = Model.Code,
                    Language = Model.Language
                }
            });
        }

        public async Task PerformJudge()
        {
            Model.Status = 1;
            var judgeServer = ServiceProvider.GetRequiredService<JudgeServer>();
            logger.LogDebug("Submitting submission id {Id}", Model.Id);
            await judgeServer.SubmitJudge(Model.Id);
        }

        public Task RequestCompleteNotification(ISubmissionCompleteCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}