using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemLocator : IObjectLocator
    {
        private readonly IServiceProvider provider;
        public StandardProblemLocator(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task<IObject> GetObject(string segment)
        {
            Guid guid;
            if(!Guid.TryParse(segment, out guid))
                return null;
            
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();
            var model = await dbContext.Set<ProblemDbModel>().FindAsync(new object[] { guid });
            if(model == null)
                return null;
            
            return new StandardProblem(provider, dbContext, model);
        }

        public Task<IObjectLocator> GetObjectLocator(string segment)
            => Task.FromResult<IObjectLocator>(null);

        public Uri GetUri()
            => new Uri("object:///Syzoj.Api/problem-standard/");
        
    }
}