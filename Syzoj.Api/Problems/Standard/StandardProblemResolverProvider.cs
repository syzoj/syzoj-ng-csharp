using System;
using System.Threading.Tasks;
using MessagePack;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemResolverProvider : IProblemResolverProvider
    {
        private readonly ApplicationDbContext context;

        public StandardProblemResolverProvider(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IProblemResolver> GetProblemResolver(Guid problemId)
        {
            var model = await context.Problems.FindAsync(problemId);
            return new StandardProblemResolver(problemId, model);
        }

        public Task<ISubmissionResolver> GetSubmissionResolver(Guid problemsetId, Guid submissionId)
        {
            throw new NotImplementedException();
        }

        private class StandardProblemResolver : IProblemResolver, IStandardProblemStatement
        {
            public StandardProblemResolver(Guid Id, Problem model)
            {
                this.Id = Id;
                this.model = model;
                this.data = MessagePackSerializer.Deserialize<StandardProblemContent>(model.Data);
            }
            public Guid Id { get; }

            private readonly Problem model;
            private readonly StandardProblemContent data;

            public Task<ProblemStatement> GetProblemStatement()
            {
                return Task.FromResult(data.Statement);
            }
        }
    }
}