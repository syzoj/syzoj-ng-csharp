using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class ProblemSubmission : DbModelObjectBase<Model.ProblemSubmission>, IProblemSubmission
    {
        public ProblemSubmission(IServiceProvider serviceProvider, ApplicationDbContext dbContext, Model.ProblemSubmission model) : base(serviceProvider, dbContext, model)
        {
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
                    ViewToken = Model.ViewToken
                }
            });
        }

        public Task RequestCompleteNotification(ISubmissionCompleteCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}