using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpFileSystem;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using MessagePack;

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
        public LegacySyzojProblemParser(IFileSystem fileSystem, ApplicationDbContext dbContext)
        {
            this.fileSystem = fileSystem;
            this.dbContext = dbContext;
        }

        public Task HandleSubmissionAsync(Problem problem, Submission submission, object data)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsProblemValidAsync(Problem problem)
        {
            return Task.Run(() => fileSystem.Exists(FileSystemPath.Parse(problem.Path).AppendFile("syzoj-export.json")));
        }

        public async Task ParseProblemAsync(Problem problem)
        {
            var obj = await Task.Run(() => {
                var stream = fileSystem.OpenFile(FileSystemPath.Parse(problem.Path).AppendFile("syzoj-export.json"), FileAccess.Read);
                var serializer = new JsonSerializer();
                using(var reader = new StreamReader(stream)) {
                    using(var jsonReader = new JsonTextReader(reader)) {
                        return serializer.Deserialize<ExportJson>(jsonReader);
                    }
                }
            });
            if(obj.success)
            {
                problem.Title = obj.obj.title;
                problem.Statement = MessagePackSerializer.Serialize(new {
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
                });
                problem.IsSubmittable = true;
            }
            await dbContext.SaveChangesAsync();
        }

        private class ExportJson
        {
            public bool success { get; set; }
            public ExportJsonObject obj { get; set; }
        }

        private class ExportJsonObject
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
    }
}