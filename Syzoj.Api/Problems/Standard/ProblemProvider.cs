using System;
using System.Threading.Tasks;
using MessagePack;
using Syzoj.Api.Data;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class ProblemProvider : DbModelObjectProviderBase<Model.Problem, Problem>
    {
        public ProblemProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task<Problem> GetObjectImpl(ApplicationDbContext dbContext, Model.Problem model)
            => Task.FromResult(new Problem(ServiceProvider, dbContext, model));
        
        public Task<Problem> CreateObject(ApplicationDbContext dbContext, Model.ProblemStatement Statement, Model.StandardTestData TestData)
            => base.CreateObject<ProblemProvider>(dbContext, new Model.Problem() {
                _Statement = MessagePackSerializer.Serialize(Statement),
                _TestData = MessagePackSerializer.Serialize(TestData)
            });
    }
}