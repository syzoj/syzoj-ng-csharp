using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces.View;
using Syzoj.Api.Object;
using Syzoj.Api.Problems;
using Syzoj.Api.Problemsets.Standard.Model;

namespace Syzoj.Api.Problemsets.Standard
{
    public class Problemset : IObject
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Model.Problemset model;
        private readonly ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider;

        private Problemset(ApplicationDbContext dbContext, Model.Problemset model, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider)
        {
            this.dbContext = dbContext;
            this.model = model;
            this.viewContractProvider = viewContractProvider;
        }
        public Guid Id => model.Id;

        public async Task AddProblem(IObject problem)
        {
            if(problem is IProblemAcceptingContract<IViewProblemsetContract, IViewProblemContract> problemWithContract)
            {
                var problemsetContract = await viewContractProvider.CreateObject(this);
                var problemContract = await problemWithContract.BuildContract(problemsetContract);
                await problemsetContract.AttachProblemContract(problemContract);
            }
            else
            {
                throw new InvalidOperationException(String.Format("Unsupported problem {Id} for standard problemset", problem.Id));
            }
        }

        public class ProblemsetViewContract : IViewProblemsetContract
        {
            private readonly ApplicationDbContext dbContext;
            private readonly Model.ProblemsetViewContract model;

            private ProblemsetViewContract(ApplicationDbContext dbContext, Model.ProblemsetViewContract model)
            {
                this.dbContext = dbContext;
                this.model = model;
            }
            public Guid Id => model.Id;

            bool IViewProblemsetContract.RequiresNotification => false;

            internal Task AttachProblemContract(IViewProblemContract problemContract)
            {
                model.ProblemContractId = problemContract.Id;
                return dbContext.SaveChangesAsync();
            }

            Task IViewProblemsetContract.OnProblemUpdated()
            {
                return Task.CompletedTask;
            }

            public class ProblemsetViewContractProvider : IObjectProvider<ProblemsetViewContract>
            {
                private readonly IServiceProvider serviceProvider;
                private readonly IObjectService service;

                public ProblemsetViewContractProvider(IServiceProvider serviceProvider, IObjectService service)
                {
                    this.serviceProvider = serviceProvider;
                    this.service = service;
                }
                
                public async Task<ProblemsetViewContract> GetObject(Guid Id)
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    var model = await dbContext.Set<Model.ProblemsetViewContract>().FindAsync(Id);
                    if(model == null)
                        return null;
                    
                    return new ProblemsetViewContract(dbContext, model);
                }

                public async Task<ProblemsetViewContract> CreateObject(Problemset problemset)
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    var id = await service.CreateObject<ProblemsetViewContractProvider>();
                    var model = new Model.ProblemsetViewContract()
                    {
                        Id = id,
                        ProblemsetId = problemset.Id
                    };
                    dbContext.Set<Model.ProblemsetViewContract>().Add(model);
                    await dbContext.SaveChangesAsync();
                    return new ProblemsetViewContract(dbContext, model);
                }

                Task<IObject> IObjectProvider.GetObject(Guid Id)
                {
                    return GetObject(Id).ContinueWith(p => (IObject) p);
                }
            }
        }

        public class ProblemsetProvider : IObjectProvider<Problemset>
        {
            private readonly IServiceProvider serviceProvider;
            private readonly IObjectService service;
            private readonly ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider;
            public ProblemsetProvider(IServiceProvider serviceProvider, IObjectService service, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider)
            {
                this.serviceProvider = serviceProvider;
                this.service = service;
                this.viewContractProvider = viewContractProvider;
            }
            public async Task<IObject> GetObject(Guid Id)
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var model = await dbContext.Set<Model.Problemset>().FindAsync(Id);
                if(model == null)
                    return null;
                
                return new Problemset(dbContext, model, viewContractProvider);
            }
        }
    }
}