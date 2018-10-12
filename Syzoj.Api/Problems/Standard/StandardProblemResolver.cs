using System;
using System.IO;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Problems.Interfaces;
using Syzoj.Api.Problems.Standard.Model;
using Syzoj.Api.Problemsets;
using Syzoj.Api.Problemsets.Interfaces;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemResolver : ProblemResolverBase, ISubmittable, IViewable
    {
        public StandardProblemResolver(IServiceProvider serviceProvider, Guid problemId) : base(serviceProvider, problemId)
        {
        }

        public async Task<StandardProblemContent> GetContent()
        {
            var content = await GetProblemContent();
            return content;
        }

        public Task<string> GenerateDownloadLink(string fileName)
        {
            var storageProvider = ServiceProvider.GetRequiredService<IAsyncFileStorageProvider>();
            return storageProvider.GenerateDownloadLink($"data/problem/{Id}/{fileName}", Path.GetFileName(fileName));
        }

        public Task<string> GenerateUploadLink(string fileName)
        {
            var storageProvider = ServiceProvider.GetRequiredService<IAsyncFileStorageProvider>();
            return storageProvider.GenerateUploadLink($"data/problem/{Id}/{fileName}");
        }

        public async Task CreateSubmissionAsync(Guid submissionId)
        {
            var redis = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var content = await this.GetProblemContent();
            await redis.GetDatabase().HashSetAsync(
                $"syzoj:problem-standard:{submissionId}:data",
                new HashEntry[] {
                    new HashEntry("status", 0),
                    new HashEntry("problemId", Id.ToString()),
                    new HashEntry("data", MessagePackSerializer.Serialize(content.TestData)),
                });
        }

        public async Task<bool> SubmitCodeAsync(Guid submissionId, string language, string code)
        {
            var redis = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var status = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problem-standard:{submissionId}:data",
                new RedisValue[] { "status", "problemId" });
            if(!status[0].HasValue || (string)status[0] != "0" || Guid.Parse((string)status[1]) != Id)
                return false;
            
            await redis.GetDatabase().HashSetAsync(
                $"syzoj:problem-standard:{submissionId}:data",
                new HashEntry[] {
                    new HashEntry("status", 1),
                    new HashEntry("language", language),
                    new HashEntry("code", code),
                });
            
            var judger = ServiceProvider.GetRequiredService<StandardProblemJudger>();
            await judger.SubmitJudge(submissionId);
            return true;
        }

        public async Task<bool> Put(StandardProblemContent content)
        {
            await SetProblemContent(content);
            return true;
        }

        public async Task<ProblemViewModel> GetProblemView()
        {
            var content = await GetProblemContent();
            return new ProblemViewModel() {
                ProblemType = "standard",
                Data = content.Statement,
            };
        }

        private async Task<StandardProblemContent> GetProblemContent()
        {
            var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var model = await dbContext.Set<Problem>().FindAsync(Id);
            return MessagePackSerializer.Deserialize<StandardProblemContent>(model.Data);
        }

        private async Task SetProblemContent(StandardProblemContent content)
        {
            var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var model = await dbContext.Set<Problem>().FindAsync(Id);
            model.Data = MessagePackSerializer.Serialize(content);
            await dbContext.SaveChangesAsync();
        }

        public override Task<bool> IsProblemsetAcceptable(IProblemsetResolver problemsetResolver)
        {
            return Task.FromResult(problemsetResolver is ISubmissionPermissionHandler);
        }
    }
}