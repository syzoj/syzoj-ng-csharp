using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpFileSystem;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using MessagePack;
using System.Collections;
using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Impl;
using RabbitMQ.Client;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Syzoj.Api.Services
{
    /// <summary>
    /// The parser class that parses legacy SYZOJ problems.
    /// The data folder needs to be like:
    /// +--- data
    /// |    +--- data.yml
    /// |    +--- data.in
    /// |    +--- data.out
    /// +--- syzoj-export.json
    ///
    /// `syzoj-export.json` is the json file from http://syzoj/problem/xxx/export.
    /// The `data` folder contains all testdata files.
    /// </summary>

    public sealed class LegacySyzojProblemParser : IAsyncProblemParser
    {
        private readonly IFileSystem fileSystem;
        private readonly ApplicationDbContext dbContext;
        private readonly IServiceProvider provider;
        private readonly ILegacySyzojJudger judger;
        public LegacySyzojProblemParser(IFileSystem fileSystem, ApplicationDbContext dbContext, IServiceProvider provider, ILegacySyzojJudger judger)
        {
            this.fileSystem = fileSystem;
            this.dbContext = dbContext;
            this.provider = provider;
            this.judger = judger;
        }

        // TODO: Handle errors, especially model errors
        public async Task HandleSubmissionAsync(Guid problemId, Guid submissionId, dynamic data)
        {
            var (metadata, path) = await dbContext.Problems
                .Where(p => p.Id == problemId)
                .Select(p => ValueTuple.Create(MessagePackSerializer.Deserialize<ProblemMetadata>(p.Metadata), p.Path))
                .FirstOrDefaultAsync();
            object param;
            object extraData = null;
            switch(metadata.Type)
            {
                case "traditional":
                    param = new LegacyJudgeRequestTraditional() {
                        language = data["Language"],
                        code = data["Code"],
                        timeLimit = metadata.TimeLimit,
                        memoryLimit = metadata.MemoryLimit,
                        fileIOInput = metadata.FileIo ? metadata.FileIoInputName : null,
                        fileIOOutput = metadata.FileIo ? metadata.FileIoOutputName : null,
                    };
                    break;
                case "submit-answer":
                    param = null;
                    extraData = data["File"];
                    break;
                case "interactive":
                    param = new LegacyJudgeRequestInteraction() {
                        language = data["Language"],
                        code = data["Code"],
                        timeLimit = metadata.TimeLimit,
                        memoryLimit = metadata.MemoryLimit,
                    };
                    break;
                default:
                    throw new ArgumentException("Invalid problem type");
            }
            LegacyJudgeRequest judgeRequest = new LegacyJudgeRequest() {
                content = new LegacyJudgeRequestContent() {
                    taskId = submissionId.ToString(),
                    testData = path + "data/",
                    type = "traditional",
                    priority = 2,
                    param = param,
                },
                extraData = null,
            };
            await judger.SendJudgeRequestAsync(2, MessagePackSerializer.Serialize(judgeRequest));
        }

        public async Task<bool> IsProblemValidAsync(Guid problemId)
        {
            var problemPath = await dbContext.Problems.Where(p => p.Id == problemId).Select(p => p.Path).FirstOrDefaultAsync();
            return await Task.Run(() => fileSystem.Exists(FileSystemPath.Parse(problemPath).AppendFile("syzoj-export.json")));
        }

        public async Task ParseProblemAsync(Guid problemId)
        {
            var problemPath = await dbContext.Problems.Where(p => p.Id == problemId).Select(p => p.Path).FirstOrDefaultAsync();
            var obj = await Task.Run(() => {
                var stream = fileSystem.OpenFile(FileSystemPath.Parse(problemPath).AppendFile("syzoj-export.json"), FileAccess.Read);
                var serializer = new JsonSerializer();
                using(var reader = new StreamReader(stream)) {
                    using(var jsonReader = new JsonTextReader(reader)) {
                        return serializer.Deserialize<ExportJson>(jsonReader);
                    }
                }
            });
            if(obj.success)
            {
                await dbContext.Problems
                    .Where(p => p.Id == problemId)
                    .UpdateAsync(p => new Problem() {
                        Title = obj.obj.title,
                        Statement = MessagePackSerializer.Serialize(new {
                            Description = obj.obj.description,
                            InputFormat = obj.obj.input_format,
                            OutputFormat = obj.obj.output_format,
                            Example = obj.obj.example,
                            LimitAndHint = obj.obj.limit_and_hint,
                            TimeLimit = obj.obj.time_limit,
                            MemoryLimit = obj.obj.memory_limit,
                            FileIo = obj.obj.file_io,
                            FileIoInputName = obj.obj.file_io_input_name,
                            FileIoOutputName = obj.obj.file_io_output_name,
                            Type = obj.obj.type,
                            Tags = obj.obj.tags,
                        }),
                        Metadata = MessagePackSerializer.Serialize(new ProblemMetadata() {
                            TimeLimit = obj.obj.time_limit,
                            MemoryLimit = obj.obj.memory_limit,
                            FileIo = obj.obj.file_io,
                            FileIoInputName = obj.obj.file_io_input_name,
                            FileIoOutputName = obj.obj.file_io_output_name,
                            Type = obj.obj.type,
                        }),
                        IsSubmittable = true,
                    });
            }
        }

        public class ExportJson
        {
            public bool success { get; set; }
            public ExportJsonObject obj { get; set; }
        }

        public class ExportJsonObject
        {
            public string title { get; set; }
            public string description { get; set; }
            public string input_format { get; set; }
            public string output_format { get; set; }
            public string example { get; set; }
            public string limit_and_hint { get; set; }
            public int time_limit { get; set; }
            public int memory_limit { get; set; }
            public bool file_io { get; set; }
            public string file_io_input_name { get; set; }
            public string file_io_output_name { get; set; }
            public string type { get; set; }
            public string[] tags { get; set; }
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public class ProblemMetadata
        {
            public int TimeLimit { get; set; }
            public int MemoryLimit { get; set; }
            public bool FileIo { get; set; }
            public string FileIoInputName { get; set; }
            public string FileIoOutputName { get; set; }
            public string Type { get; set; }
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public class LegacyJudgeRequest
        {
            public LegacyJudgeRequestContent content { get; set; }
            public object extraData { get; set; }
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public class LegacyJudgeRequestContent
        {
            public string taskId { get; set; }
            public string testData { get; set; }
            public string type { get; set; }
            public int priority { get; set; }
            public object param { get; set; }
        }
        
        [MessagePackObject(keyAsPropertyName: true)]
        public class LegacyJudgeRequestTraditional
        {
            public string language { get; set; }
            public string code { get; set; }
            public int timeLimit { get; set; }
            public int memoryLimit { get; set; }
            public string fileIOInput { get; set; }
            public string fileIOOutput { get; set; }
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public class LegacyJudgeRequestInteraction
        {
            public string language { get; set; }
            public string code { get; set; }
            public int timeLimit { get; set; }
            public int memoryLimit { get; set; }
        }
    }
}