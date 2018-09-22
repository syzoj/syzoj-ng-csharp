using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using StackExchange.Redis;
using Syzoj.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Z.EntityFramework.Plus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Syzoj.Api.Services
{
    // FIXME: The implementation has very poor concurrency guarantees.
    // TODO: The redis DEL operation can block other clients. Try not doing
    // it atomically.
    public class DefaultProblemsetManager : IAsyncProblemsetManager
    {
        private ApplicationDbContext dbContext;
        private IConnectionMultiplexer redis;

        public DefaultProblemsetManager(ApplicationDbContext dbContext, IConnectionMultiplexer redis)
        {
            this.dbContext = dbContext;
            this.redis = redis;
        }

        public async Task AttachProblem(Guid problemsetId, Guid problemId, string name)
        {
            dbContext.ProblemsetProblems
                .Add(new ProblemsetProblem() {
                    ProblemsetId = problemsetId,
                    ProblemId = problemId,
                    ProblemsetProblemId = name,
                });
            await dbContext.SaveChangesAsync();
            
            #pragma warning disable CS4014
            var tran = redis.GetDatabase().CreateTransaction();
            tran.SortedSetAddAsync(
                key: $"syzoj:problemset:{problemsetId}:sort:name",
                member: name + problemId,
                score: 0,
                flags: CommandFlags.FireAndForget
            );
            tran.HashSetAsync(
                key: $"syzoj:problemset:{problemsetId}:problem_id",
                hashFields: new HashEntry[] { new HashEntry(name, problemId.ToString()) },
                flags: CommandFlags.FireAndForget
            );
            tran.HashSetAsync(
                key: $"syzoj:problemset:{problemsetId}:problem_name",
                hashFields: new HashEntry[] { new HashEntry(problemId.ToString(), name) },
                flags: CommandFlags.FireAndForget
            );
            #pragma warning restore CS4014
            bool success = await tran.ExecuteAsync();
            if(!success)
                throw new Exception("Redis transaction failed");
        }

        public async Task ChangeProblemName(Guid problemsetId, Guid problemId, string newName)
        {
            var problemsetProblem = await dbContext.ProblemsetProblems
                .FindAsync(new object[] { problemsetId, problemId });
            var oldName = problemsetProblem.ProblemsetProblemId;
            problemsetProblem.ProblemsetProblemId = newName;
            await dbContext.SaveChangesAsync();
            
            #pragma warning disable CS4014
            var tran = redis.GetDatabase().CreateTransaction();
            tran.SortedSetRemoveAsync(
                key: $"syzoj:problemset:{problemsetId}:sort:name",
                member: oldName + problemId,
                flags: CommandFlags.FireAndForget
            );
            tran.SortedSetAddAsync(
                key: $"syzoj:problemset:{problemsetId}:sort:name",
                member: newName + problemId,
                score: 0,
                flags: CommandFlags.FireAndForget
            );
            tran.HashDeleteAsync(
                key: $"syzoj:problemset:{problemsetId}:problem_id",
                hashField: oldName,
                flags: CommandFlags.FireAndForget
            );
            tran.HashSetAsync(
                key: $"syzoj:problemset:{problemsetId}:problem_id",
                hashFields: new HashEntry[] { new HashEntry(newName, problemId.ToString()) },
                flags: CommandFlags.FireAndForget
            );
            tran.HashSetAsync(
                key: $"syzoj:problemset:{problemsetId}:problem_name",
                hashFields: new HashEntry[] { new HashEntry(problemId.ToString(), newName) },
                flags: CommandFlags.FireAndForget
            );
            #pragma warning restore CS4014
            bool success = await tran.ExecuteAsync();
            if(!success)
                throw new Exception("Redis transaction failed");
        }

        public Task DetachProblem(Guid problemsetId, Guid problemId)
        {
            throw new NotImplementedException();
        }

        public Task DoGarbageCollect()
        {
            return Task.CompletedTask;
        }

        public async Task NewSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission)
        {
            dbContext.Submissions
                .Add(new Submission() {
                    Id = submissionId,
                    ProblemsetId = problemsetId,
                    ProblemId = problemId,
                    Content = JsonConvert.SerializeObject(submission),
                });
            await dbContext.SaveChangesAsync();
        }

        private async Task InvalidateProblemCache(Guid problemId)
        {
            #pragma warning disable CS4014
            var tran = redis.GetDatabase().CreateTransaction();
            tran.KeyDeleteAsync($"syzoj:problem:{problemId}:cache");
            tran.HashSetAsync($"syzoj:problem:{problemId}:cache", new HashEntry[] {
                new HashEntry("cache", Guid.NewGuid().ToString())
            });
            #pragma warning restore CS4014
            bool success = await tran.ExecuteAsync();
            if(!success)
                throw new Exception("Redis transaction failed");
        }

        private async Task InvalidateSubmissionCache(Guid submissionId)
        {
            #pragma warning disable CS4014
            var tran = redis.GetDatabase().CreateTransaction();
            tran.KeyDeleteAsync($"syzoj:submission:{submissionId}:cache");
            tran.HashSetAsync($"syzoj:submission:{submissionId}:cache", new HashEntry[] {
                new HashEntry("cache", Guid.NewGuid().ToString())
            });
            #pragma warning restore CS4014
            bool success = await tran.ExecuteAsync();
            if(!success)
                throw new Exception("Redis transaction failed");
        }

        public async Task PatchProblem(Guid problemsetId, Guid problemId, object problemPatch)
        {
            var problem = await dbContext.Problems.FindAsync(problemId);
            JObject orgProblem = JObject.Parse(problem.Content);
            orgProblem.Merge(problemPatch);
            problem.Content = orgProblem.ToString();
            await dbContext.SaveChangesAsync();
            await InvalidateProblemCache(problemId);
        }

        public Task PatchSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission)
        {
            throw new NotImplementedException();
        }

        public async Task PutProblem(Guid problemsetId, Guid problemId, object problem)
        {
            var problemContent = JsonConvert.SerializeObject(problem);
            /*await dbContext.Problems
                .Where(p => p.Id == problemId)
                .UpdateAsync(p => new Problem() { Content = problemContent });*/
            var problemObj = await dbContext.Problems.FindAsync(new object[] { problemId });
            problemObj.Content = problemContent;
            await dbContext.SaveChangesAsync();
            
            await InvalidateProblemCache(problemId);
        }

        public async Task PutSubmission(Guid problemsetId, Guid problemId, Guid submissionId, object submission)
        {
            var submissionContent = JsonConvert.SerializeObject(submission);
            await dbContext.Submissions
                .Where(s => s.ProblemsetId == problemsetId && s.ProblemId == problemId && s.Id == submissionId)
                .UpdateAsync(s => new Submission { Content = submissionContent });

            await InvalidateSubmissionCache(submissionId);
        }

        public Task RebuildIndex()
        {
            throw new NotImplementedException();
        }
    }
}