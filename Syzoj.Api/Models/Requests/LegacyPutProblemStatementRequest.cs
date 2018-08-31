using Syzoj.Api.Models;

namespace Syzoj.Api.Requests
{
    public class LegacyPutProblemStatementRequest
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Example { get; set; }
        public string LimitAndHint { get; set; }
        public string[] Tags { get; set; }
    }
}