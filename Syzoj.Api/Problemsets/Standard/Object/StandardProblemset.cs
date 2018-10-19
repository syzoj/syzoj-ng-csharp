using System;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Problemsets.Standard.Model;

namespace Syzoj.Api.Problemsets.Standard.Object
{
    public class StandardProblemset : IProblemsetObject
    {
        private IServiceProvider provider;
        private ApplicationDbContext dbContext;
        private ProblemsetDbModel model;

        public StandardProblemset(IServiceProvider provider, ApplicationDbContext dbContext, ProblemsetDbModel model)
        {
            this.provider = provider;
            this.dbContext = dbContext;
            this.model = model;
        }

        public Uri GetUri() => new Uri($"object:///Syzoj.Api/problemset-standard/problemset/{model.Id}");
    }
}