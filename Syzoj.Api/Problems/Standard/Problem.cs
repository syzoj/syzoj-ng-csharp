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
    public class Problem : DbModelObjectBase<Model.Problem>, IProblemAcceptingContract<IViewProblemsetContract, IViewProblemContract>
    {
        private readonly ProblemViewContract.ProblemViewContractProvider viewContractProvider;
        private Problem(ApplicationDbContext dbContext, Model.Problem model, ProblemViewContract.ProblemViewContractProvider viewContractProvider)
            : base(dbContext, model)
        {
            this.viewContractProvider = viewContractProvider;
        }

        async Task<IViewProblemContract> IProblemAcceptingContract<IViewProblemsetContract, IViewProblemContract>.BuildContract(IViewProblemsetContract contract)
        {
            return (IViewProblemContract) await viewContractProvider.CreateObject(Model.Id, contract.Id);
        }

        private Task<ProblemStatement> GetStatement()
            => Task.FromResult(MessagePack.MessagePackSerializer.Deserialize<ProblemStatement>(Model._Statement));

        public class ProblemProvider : DbModelObjectBase<Model.Problem>.Provider<Problem, ProblemProvider>
        {
            private readonly ProblemViewContract.ProblemViewContractProvider viewContractProvider;

            public ProblemProvider(IServiceProvider serviceProvider, ProblemViewContract.ProblemViewContractProvider viewContractProvider)
                : base(serviceProvider)
            {
                this.viewContractProvider = viewContractProvider;
            }

            protected override Task<Problem> GetObjectImpl(ApplicationDbContext dbContext, Model.Problem model)
            {
                return Task.FromResult(new Problem(dbContext, model, viewContractProvider));
            }

            public Task<Problem> CreateObject(ProblemStatement statement)
            {
                return base.CreateObject(new Model.Problem()
                {
                    _Statement = MessagePack.MessagePackSerializer.Serialize(statement)
                });
            }
        }

        public class ProblemViewContract : DbModelObjectBase<Model.ProblemViewContract>, IViewProblemContract
        {
            private Problem problem;
            private readonly IObjectService objectService;

            public ProblemViewContract(ApplicationDbContext dbContext, Model.ProblemViewContract model, IObjectService objectService)
                : base(dbContext, model)
            {
                this.objectService = objectService;
            }

            private async Task OnLoad()
            {
                this.problem = (Problem) await objectService.GetObject(Model.ProblemId);
            }

            public async Task CancelContract()
            {
                DbContext.Set<Model.ProblemViewContract>().Remove(Model);
                await DbContext.SaveChangesAsync();
                await objectService.DeleteObject(Model.Id);
            }

            public async Task<ViewModel> GetProblemStatement()
            {
                return new ViewModel()
                {
                    ComponentName = "problem-standard",
                    Content = await problem.GetStatement()
                };
            }

            public class ProblemViewContractProvider : DbModelObjectBase<Model.ProblemViewContract>.Provider<ProblemViewContract, ProblemViewContractProvider>
            {
                public ProblemViewContractProvider(IServiceProvider serviceProvider) : base(serviceProvider)
                {
                }

                protected override async Task<ProblemViewContract> GetObjectImpl(ApplicationDbContext dbContext, Model.ProblemViewContract model)
                {
                    var obj = new ProblemViewContract(dbContext, model, ObjectService);
                    await obj.OnLoad();
                    return obj;
                }

                public Task<ProblemViewContract> CreateObject(Guid problemId, Guid problemsetContractId)
                {
                    return base.CreateObject(new Model.ProblemViewContract()
                    {
                        ProblemId = problemId,
                        ProblemsetContractId = problemsetContractId
                    });
                }
            }
        }
    }
}