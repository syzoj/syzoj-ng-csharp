namespace Syzoj.Api.Models.Data
{
    public class ProblemSetProblem
    {
        public virtual ProblemSet ProblemSet { get; set; }
        public int ProblemSetId { get; set; }
        public virtual Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public string ProblemSetProblemId { get; set; }
        // Number of submissions.
        public int Submissions { get; set; }
        // Number of Accepted submissions.
        public int Accepts { get; set; }
    }
}