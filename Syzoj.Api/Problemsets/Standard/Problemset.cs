using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
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
        private readonly ILogger<Problemset> logger;

        private Problemset(ApplicationDbContext dbContext, Model.Problemset model, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider, IObjectService service, ILoggerFactory loggerFactory)
            : base(dbContext, model)
        {
            this.viewContractProvider = viewContractProvider;
            this.service = service;
            this.logger = loggerFactory.CreateLogger<Problemset>();
        }

        public async Task<bool> AddProblem(IObject problem, string identifier)
        {
            if(problem is IProblemAcceptingContract<IViewProblemsetContract, IViewProblemContract> problemWithContract)
            {
                var problemsetContract = await viewContractProvider.CreateObject(this, identifier);
                var problemContract = await problemWithContract.BuildContract(problemsetContract);
                await problemsetContract.AttachProblemContract(problemContract);
                return true;
            }
            else
            {
                return false;
            }
        }

        public class ProblemSummary
        {
            public Guid Id { get; set; }
            public string Identifier { get; set; }
            public string Title { get; set; }
        }
        public async Task<IEnumerable<ProblemSummary>> GetProblems()
        {
            var contracts = await DbContext.Entry(Model).Collection(ps => ps.ViewContracts).Query()
                .ToListAsync();
            var result = new List<ProblemSummary>();
            foreach(var model in contracts)
            {
                var contract = await service.GetObject(model.ProblemContractId.Value) as IViewProblemContract;
                if(contract != null)
                {
                    var title = model.Title ?? await contract.GetProblemDefaultTitle();
                    result.Add(new ProblemSummary() {
                        Id = model.Id,
                        Identifier = model.Identifier,
                        Title = title
                    });
                }
            }
            return result;
        }

        public class ProblemContent
        {
            public Guid Id { get; set; }
            public string Identifier { get; set; }
            public string Title { get; set; }
            public ViewModel Content { get; set; }
        }
        public async Task<ProblemContent> GetProblem(string identifier)
        {
            var entry = await DbContext.Entry(Model).Collection(ps => ps.ViewContracts).Query()
                .FirstOrDefaultAsync(c => c.Identifier == identifier);
            if(entry == null || entry.ProblemContractId == null)
                return null;

            var contract = await service.GetObject(entry.ProblemContractId.Value) as IViewProblemContract;
            if(contract == null)
            {
                logger.LogWarning("Standard Problemset {id} has a corrupt problem entry: {contractId}", Model.Id, entry);
                return null;
            }

            return new ProblemContent()
            {
                Id = entry.Id,
                Identifier = entry.Identifier,
                Title = entry.Title ?? await contract.GetProblemDefaultTitle(),
                Content = await contract.GetProblemStatement(),
            };
        }

        public class ProblemsetViewContract : DbModelObjectBase<Model.ProblemsetViewContract>, IViewProblemsetContract
        {
            private ProblemsetViewContract(ApplicationDbContext dbContext, Model.ProblemsetViewContract model)
                : base(dbContext, model)
            {
            }

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

                public Task<ProblemsetViewContract> CreateObject(Problemset problemset, string identifier)
                {
                    return base.CreateObject(new Model.ProblemsetViewContract() {
                        ProblemsetId = problemset.Id,
                        Identifier = identifier
                    });
                }
            }
        }

        public class ProblemsetProvider : DbModelObjectBase<Model.Problemset>.Provider<Problemset, ProblemsetProvider>
        {
            private readonly ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider;
            private readonly ILoggerFactory loggerFactory;

            public ProblemsetProvider(IServiceProvider serviceProvider, ProblemsetViewContract.ProblemsetViewContractProvider viewContractProvider, ILoggerFactory loggerFactory)
                : base(serviceProvider)
            {
                this.viewContractProvider = viewContractProvider;
                this.loggerFactory = loggerFactory;
            }

            protected override Task<Problemset> GetObjectImpl(ApplicationDbContext dbContext, Model.Problemset model)
                => Task.FromResult(new Problemset(dbContext, model, viewContractProvider, ObjectService, loggerFactory));

            public Task<Problemset> CreateObject()
            {
                return base.CreateObject(new Model.Problemset());
            }
        }
    }
}