using System.Threading.Tasks;
using SharpFileSystem;
using Syzoj.Api.Models;

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

    public class LegacySyzojProblemParser : IAsyncProblemParser
    {
        private readonly IFileSystem fileSystem;
        public LegacySyzojProblemParser(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public Task HandleSubmissionAsync(Problem problem, Submission submission, object data)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsProblemValidAsync(Problem problem)
        {
            return Task.Run(() => fileSystem.Exists(FileSystemPath.Parse(problem.Path).AppendFile("syzoj-export.json")));
        }

        public Task ParseProblemAsync(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}