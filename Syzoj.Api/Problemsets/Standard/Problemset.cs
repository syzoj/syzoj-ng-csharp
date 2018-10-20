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
    public class Problemset : DbModelObjectBase<Model.Problemset>
    {
        private readonly ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider;
        private readonly IObjectService service;

        private Problemset(ApplicationDbContext dbContext, Model.Problemset model, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider, IObjectService service)
            : base(dbContext, model)
        {
            this.viewContractProvider = viewContractProvider;
            this.service = service;
        }

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
            var contracts = await DbContext.Entry(Model).Collection(ps => ps.ViewContracts).Query()
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

        public class ProblemsetViewContract : DbModelObjectBase<Model.ProblemsetViewContract>, IViewProblemsetContract
        {
            private ProblemsetViewContract(ApplicationDbContext dbContext, Model.ProblemsetViewContract model)
                : base(dbContext, model)
            {
            }

            bool IViewProblemsetContract.RequiresNotification => false;

            internal Task AttachProblemContract(IViewProblemContract problemContract)
            {
                Model.ProblemContractId = problemContract.Id;
                return DbContext.SaveChangesAsync();
            }

            Task IViewProblemsetContract.OnProblemUpdated()
            {
                return Task.CompletedTask;
            }

            public class ProblemsetViewContractProvider : DbModelObjectBase<Model.ProblemsetViewContract>.Provider<ProblemsetViewContract, ProblemsetViewContractProvider>
            {
                public ProblemsetViewContractProvider(IServiceProvider serviceProvider)
                    : base(serviceProvider)
                {
                }

                protected override Task<ProblemsetViewContract> GetObjectImpl(ApplicationDbContext dbContext, Model.ProblemsetViewContract model)
                    => Task.FromResult(new ProblemsetViewContract(dbContext, model));

                public Task<ProblemsetViewContract> CreateObject(Problemset problemset)
                {
                    return base.CreateObject(new Model.ProblemsetViewContract() {
                        ProblemsetId = problemset.Id
                    });
                }
            }
        }

        public class ProblemsetProvider : DbModelObjectBase<Model.Problemset>.Provider<Problemset, ProblemsetProvider>
        {
            private readonly ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider;
            public ProblemsetProvider(IServiceProvider serviceProvider, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider)
                : base(serviceProvider)
            {
                this.viewContractProvider = viewContractProvider;
            }

            protected override Task<Problemset> GetObjectImpl(ApplicationDbContext dbContext, Model.Problemset model)
                => Task.FromResult(new Problemset(dbContext, model, viewContractProvider, ObjectService));

            public Task<Problemset> CreateObject()
            {
                return base.CreateObject(new Model.Problemset());
            }
        }
    }
}