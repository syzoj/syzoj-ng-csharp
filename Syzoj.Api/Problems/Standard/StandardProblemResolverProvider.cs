using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemResolverProvider : IProblemResolverProvider
    {
        public string ProblemType => "standard";

        public async Task<IProblemResolver> CreateProblem(IServiceProvider serviceProvider, Guid problemId)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var problem = await context.Problems.FindAsync(problemId);
            var problemData = new StandardProblemContent();
            problem.Data = MessagePackSerializer.Serialize(problemData);
            await context.SaveChangesAsync();
            return new StandardProblemResolver(serviceProvider, problemId);
        }

        public Task<IProblemResolver> GetProblemResolver(IServiceProvider serviceProvider, Guid problemId)
        {
            return Task.FromResult<IProblemResolver>(new StandardProblemResolver(serviceProvider, problemId));
        }
    }
}