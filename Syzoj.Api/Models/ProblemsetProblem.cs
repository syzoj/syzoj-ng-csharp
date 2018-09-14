using System.Collections.Generic;

namespace Syzoj.Api.Models
{
    public class ProblemsetProblem
    {
        public int ProblemsetId { get; set; }
        public Problemset Problemset { get; set; }
        public int ProblemId { get; set; }
        public Problem Problem { get; set; }
        public string ProblemsetProblemId { get; set; }
    }
}