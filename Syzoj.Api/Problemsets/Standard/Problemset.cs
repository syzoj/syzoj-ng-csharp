using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Object;
using Syzoj.Api.Problemsets.Standard.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Syzoj.Api.Problemsets.Standard
{
    public class Problemset : DbModelObjectBase<Model.Problemset>
    {
        private readonly IObjectService objectService;
        private readonly ILogger<Problemset> logger;

        public Problemset(IServiceProvider serviceProvider, ApplicationDbContext dbContext, Model.Problemset model) : base(serviceProvider, dbContext, model)
        {
            this.objectService = serviceProvider.GetRequiredService<IObjectService>();
            this.logger = serviceProvider.GetRequiredService<ILogger<Problemset>>();
        }

        public async Task<Guid> AddProblem(IProblem problem, string identifier)
        {
            var contract = await problem.CreateProblemContract();
            var entryId = Guid.NewGuid();
            DbContext.Add(new Model.ProblemsetProblem() {
                Id = entryId,
                ProblemsetId = Model.Id,
                ProblemContractId = contract.Id,
                Identifier = identifier,
                Title = "Untitled"
            });
            return entryId;
        }

        public class ProblemSummary
        {
            public Guid EntryId { get; set; }
            public string Identifier { get; set; }
            public string Title { get; set; }
        }
        public async Task<IEnumerable<ProblemSummary>> GetProblems()
        {
            var problems = await DbContext.Set<Model.ProblemsetProblem>()
                .Where(p => p.ProblemsetId == Model.Id)
                .Select(ps => new ProblemSummary() {
                    EntryId = ps.Id,
                    Title = ps.Title,
                    Identifier = ps.Identifier
                })
                .ToListAsync();
            return problems;
        }

        private async Task<IProblemContract> GetProblem(Guid entryId)
        {
            var model = await DbContext.FindAsync<Model.ProblemsetProblem>(entryId);
            if(model.ProblemsetId != Model.Id)
                return null;
            
            var problemContract = await objectService.GetObject(DbContext, model.ProblemContractId) as IProblemContract;
            return problemContract;
        }

        public async Task<Guid?> GetProblemEntryId(string identifier)
        {
            var model = await DbContext.Set<ProblemsetProblem>()
                .Where(ps => ps.ProblemsetId == Model.Id && ps.Identifier == identifier)
                .FirstOrDefaultAsync();
            if(model == null)
                return null;
            
            return model.Id;
        }

        public async Task<ViewModel> GetProblemContent(Guid entryId)
        {
            var problemContract = await GetProblem(entryId);
            if(problemContract == null)
                return null;
            
            return await problemContract.GetProblemContent();
        }

        public async Task<Guid?> Submit(Guid entryId, string token)
        {
            var problemContract = await GetProblem(entryId);
            if(problemContract == null)
                return null;
            
            var submission = await problemContract.ClaimSubmission(token);
            if(submission == null)
                return null;
            
            var model = new ProblemsetSubmission() {
                Id = Guid.NewGuid(),
                ProblemsetId = Model.Id,
                SubmissionContractId = submission.Id
            };
            DbContext.Add(model);
            return model.Id;
        }

        public async Task<ViewModel> GetSubmissionContent(Guid entryId)
        {
            var model = await DbContext.FindAsync<ProblemsetSubmission>(entryId);
            if(model == null)
                return null;

            if(model.ProblemsetId != Model.Id)
                return null;
            
            var contract = await objectService.GetObject(DbContext, model.SubmissionContractId) as IProblemSubmission;
            if(contract == null)
                return null;
            
            return await contract.GetSubmissionContent();
        }
    }
}