using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Syzoj.Api.Data;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardProblemResolver : ProblemResolverBase, ISubmittable
    {
        public StandardProblemResolver(IServiceProvider serviceProvider, Guid problemId) : base(serviceProvider, problemId)
        {
        }

        public async Task<StandardProblemContent> GetContent()
        {
            var model = await GetProblemModel();
            var content = MessagePackSerializer.Deserialize<StandardProblemContent>(model.Data);
            return content;
        }

        public Task<string> GenerateDownloadLink(Guid fileId, string fileName)
        {
            var storageProvider = ServiceProvider.GetRequiredService<IAsyncFileStorageProvider>();
            return storageProvider.GenerateDownloadLink($"data/problem/{Id}/{fileId}", fileName);
        }

        public Task<string> GenerateUploadLink(Guid fileId)
        {
            var storageProvider = ServiceProvider.GetRequiredService<IAsyncFileStorageProvider>();
            return storageProvider.GenerateUploadLink($"data/problem/{Id}/{fileId}");
        }

        public Task CreateSubmissionAsync(Guid submissionId)
        {
            var redis = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            return redis.GetDatabase().HashSetAsync(
                $"syzoj:problem:{Id}:submission:{submissionId}:data",
                new HashEntry[] { new HashEntry("status", 0) });
        }

        public async Task<bool> SubmitCodeAsync(Guid submissionId, string language, string code)
        {
            var redis = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var status = await redis.GetDatabase().HashGetAsync(
                $"syzoj:problem:{Id}:submission:{submissionId}:data",
                "status");
            if(!status.HasValue || (string)status != "0")
                return false;
            
            await redis.GetDatabase().HashSetAsync(
                $"syzoj:problem:{Id}:submission:{submissionId}:data",
                new HashEntry[] {
                    new HashEntry("status", 1),
                    new HashEntry("language", language),
                    new HashEntry("code", code),
                });
            
            var judger = ServiceProvider.GetRequiredService<StandardProblemJudger>();
            await judger.SubmitJudge(submissionId);
            return true;
        }
    }
}