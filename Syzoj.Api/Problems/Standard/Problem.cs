using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces.View;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class Problem : IProblemAcceptingContract<IViewProblemsetContract, IViewProblemContract>
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Model.Problem model;
        private readonly ProblemViewContract.ProblemViewContractProvider viewContractProvider;

        public Guid Id => model.Id;
        private Problem(ApplicationDbContext dbContext, Model.Problem model, ProblemViewContract.ProblemViewContractProvider viewContractProvider)
        {
            this.dbContext = dbContext;
            this.model = model;
            this.viewContractProvider = viewContractProvider;
        }

        async Task<IViewProblemContract> IProblemAcceptingContract<IViewProblemsetContract, IViewProblemContract>.BuildContract(IViewProblemsetContract contract)
        {
            return (IViewProblemContract) await viewContractProvider.CreateObject(model.Id, contract.Id);
        }

        public class ProblemProvider : IObjectProvider<Problem>
        {
            private readonly IServiceProvider serviceProvider;
            private readonly IObjectService service;
            private readonly ProblemViewContract.ProblemViewContractProvider viewContractProvider;

            public ProblemProvider(IServiceProvider serviceProvider, IObjectService service, ProblemViewContract.ProblemViewContractProvider viewContractProvider)
            {
                this.serviceProvider = serviceProvider;
                this.service = service;
                this.viewContractProvider = viewContractProvider;
            }

            public async Task<Problem> GetObject(Guid Id)
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var model = await dbContext.Set<Model.Problem>().FindAsync(Id);
                if(model == null)
                    return null;
                
                return new Problem(dbContext, model, viewContractProvider);
            }

            public async Task<Problem> CreateObject(ProblemStatement statement)
            {
                if(statement == null)
                    throw new ArgumentNullException(nameof(statement));

                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var id = await service.CreateObject<ProblemProvider>();
                var model = new Model.Problem()
                {
                    Id = id,
                    _Statement = MessagePack.MessagePackSerializer.Serialize(statement)
                };
                dbContext.Set<Model.Problem>().Add(model);
                return new Problem(dbContext, model, viewContractProvider);
            }

            Task<IObject> IObjectProvider.GetObject(Guid Id)
            {
                return GetObject(Id).ContinueWith(p => (IObject) p);
            }
        }

        public class ProblemViewContract : IViewProblemContract
        {
            private readonly ApplicationDbContext dbContext;
            private readonly Model.ProblemViewContract model;

            private ProblemViewContract(ApplicationDbContext dbContext, Model.ProblemViewContract model)
            {
                this.dbContext = dbContext;
                this.model = model;
            }
            public Guid Id => model.Id;

            public async Task<ViewModel> GetProblemStatement()
            {
                var problem = await dbContext.Set<Model.Problem>().FindAsync(model.ProblemId);
                return new ViewModel()
                {
                    ComponentName = "problem-standard",
                    Content = MessagePack.MessagePackSerializer.Deserialize<ProblemStatement>(problem._Statement)
                };
            }

            public class ProblemViewContractProvider : IObjectProvider<ProblemViewContract>
            {
                private readonly IServiceProvider serviceProvider;
                private readonly IObjectService service;

                public ProblemViewContractProvider(IServiceProvider serviceProvider, IObjectService service)
                {
                    this.serviceProvider = serviceProvider;
                    this.service = service;
                }

                public async Task<ProblemViewContract> GetObject(Guid Id)
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    var model = await dbContext.Set<Model.ProblemViewContract>().FindAsync(Id);
                    if(model == null)
                        return null;
                    
                    return new ProblemViewContract(dbContext, model);
                }

                public async Task<ProblemViewContract> CreateObject(Guid problemId, Guid problemsetContractId)
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    var id = await service.CreateObject<ProblemViewContractProvider>();
                    var model = new Model.ProblemViewContract()
                    {
                        Id = id,
                        ProblemId = problemId,
                        ProblemsetContractId = problemsetContractId
                    };
                    dbContext.Set<Model.ProblemViewContract>().Add(model);
                    await dbContext.SaveChangesAsync();
                    return new ProblemViewContract(dbContext, model);
                }

                Task<IObject> IObjectProvider.GetObject(Guid Id)
                {
                    return GetObject(Id).ContinueWith(pv => (IObject) pv);
                }
            }
        }
    }
}