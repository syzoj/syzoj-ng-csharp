using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class ProblemSubmissionProvider : DbModelObjectProviderBase<Model.ProblemSubmission, ProblemSubmission>
    {
        public ProblemSubmissionProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task<ProblemSubmission> GetObjectImpl(ApplicationDbContext dbContext, Model.ProblemSubmission model)
            => Task.FromResult(new ProblemSubmission(ServiceProvider, dbContext, model));
    }
}