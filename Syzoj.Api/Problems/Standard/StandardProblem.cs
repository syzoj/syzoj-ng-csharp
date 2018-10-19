using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblem : IObject
    {
        private readonly IServiceProvider provider;
        private readonly ApplicationDbContext dbContext;
        private readonly ProblemDbModel model;
        private readonly Uri uri;

        internal StandardProblem(IServiceProvider provider, ApplicationDbContext dbContext, ProblemDbModel model)
        {
            this.provider = provider;
            this.dbContext = dbContext;
            this.model = model;
            this.uri = new Uri($"object:///Syzoj.Api/problem-standard/${model.Id}");
        }

        public Uri GetUri() => uri;
    }
}