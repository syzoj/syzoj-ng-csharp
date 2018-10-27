using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Data;
using Syzoj.Api.Object;
using Syzoj.Api.Problemsets.Standard.Model;

namespace Syzoj.Api.Problemsets.Standard
{
    public class ProblemsetProvider : DbModelObjectProviderBase<Model.Problemset, Problemset>
    {
        private readonly ILoggerFactory loggerFactory;

        public ProblemsetProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        }

        protected override Task<Problemset> GetObjectImpl(ApplicationDbContext dbContext, Model.Problemset model)
            => Task.FromResult(new Problemset(ServiceProvider, dbContext, model));

        public Task<Problemset> CreateObject(ApplicationDbContext dbContext)
            => base.CreateObject<ProblemsetProvider>(dbContext, new Model.Problemset());
    }
}