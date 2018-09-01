using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Models.Responses
{
    public class ProblemListEntryResponse
    {
        public string ProblemSetProblemId { get; set; }
        public string Title { get; set; }
        public ProblemType? Type { get; set; }
        public int? Submissions { get; set; }
        public int? Accepts { get; set; }
    }
}