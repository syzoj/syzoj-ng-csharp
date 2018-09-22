using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Syzoj.Api.Services
{
    public class DefaultProblemParser : IAsyncProblemParser
    {
        private readonly ApplicationDbContext dbContext;

        public DefaultProblemParser(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<object> GetProblemDescriptionAsync(Guid problemId)
        {
            var val = await dbContext.Problems
                .Where(p => p.Id == problemId)
                .Select(p => p.Content)
                .FirstOrDefaultAsync();
            // TODO: Is it appropriate to catch all exceptions here?
            try
            {
                var obj = JsonConvert.DeserializeObject<ProblemContent>(val);
                return obj.Statement;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<object> GetProblemSummaryAsync(Guid problemId)
        {
            var val = await dbContext.Problems
                .Where(p => p.Id == problemId)
                .Select(p => p.Content)
                .FirstOrDefaultAsync();
            // TODO: Is it appropriate to catch all exceptions here?
            try
            {
                var obj = JsonConvert.DeserializeObject<ProblemContent>(val);
                return obj.Title;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task HandleSubmissionAsync(Guid problemId, Guid submissionId)
        {
            throw new NotImplementedException();
        }

        public class ProblemContent
        {
            public string Title { get; set; }
            public object Statement { get; set; }
            public object JudgeSettings { get; set; }
        }
    }
}