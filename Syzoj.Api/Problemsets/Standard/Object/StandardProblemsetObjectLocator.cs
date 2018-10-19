using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Object;
using Syzoj.Api.Problemsets.Standard.Model;

namespace Syzoj.Api.Problemsets.Standard.Object
{
    public class StandardProblemsetObjectLocator : IObjectLocator
    {
        private readonly IServiceProvider provider;

        public StandardProblemsetObjectLocator(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public async Task<IObject> GetObject(string segment)
        {
            Guid guid;
            if(!Guid.TryParse(segment, out guid))
                return null;
            
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();
            var model = await dbContext.Set<ProblemsetDbModel>().FindAsync(new object[] { guid });
            if(model == null)
                return null;
            
            return new StandardProblemset(provider, dbContext, model);
        }

        public Task<IObjectLocator> GetObjectLocator(string segment)
        {
            return Task.FromResult<IObjectLocator>(null);
        }

        public Uri GetUri() => new Uri("object:///problemset-standard/problemset/");
    }
}