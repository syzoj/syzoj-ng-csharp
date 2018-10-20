using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        private readonly IObjectService service;

        private Problemset(ApplicationDbContext dbContext, Model.Problemset model, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider, IObjectService service)
        {
            this.dbContext = dbContext;
            this.model = model;
            this.viewContractProvider = viewContractProvider;
            this.service = service;
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

        public async Task<IEnumerable<ViewModel>> GetProblems()
        {
            var contracts = await dbContext.Entry(model).Collection(ps => ps.ViewContracts).Query()
                .Select(c => c.ProblemContractId)
                .ToListAsync();
            var result = new List<ViewModel>();
            foreach(var id in contracts)
            {
                var contract = await service.GetObject(id.Value) as IViewProblemContract;
                if(contract != null)
                    result.Add(await contract.GetProblemStatement());
            }
            return result;
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
                    var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var model = await dbContext.Set<Model.ProblemsetViewContract>().FindAsync(Id);
                    if(model == null)
                        return null;
                    
                    return new ProblemsetViewContract(dbContext, model);
                }

                public async Task<ProblemsetViewContract> CreateObject(Problemset problemset)
                {
                    var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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
                    return GetObject(Id).ContinueWith(p => (IObject) p.Result);
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
            public async Task<Problemset> GetObject(Guid Id)
            {
                var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var model = await dbContext.FindAsync<Model.Problemset>(Id);;
                if(model == null)
                    return null;
                
                return new Problemset(dbContext, model, viewContractProvider, service);
            }

            public async Task<Problemset> CreateObject()
            {
                var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var id = await service.CreateObject<ProblemsetProvider>();
                var model = new Model.Problemset()
                {
                    Id = id
                };
                dbContext.Add<Model.Problemset>(model);
                await dbContext.SaveChangesAsync();
                return new Problemset(dbContext, model, viewContractProvider, service);
            }

            Task<IObject> IObjectProvider.GetObject(Guid Id)
            {
                return GetObject(Id).ContinueWith(ps => (IObject) ps.Result);
            }
        }
    }
}