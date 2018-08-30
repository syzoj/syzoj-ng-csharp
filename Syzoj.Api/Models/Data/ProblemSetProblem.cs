using System.Collections.Generic;

namespace Syzoj.Api.Models.Data
{
    public class ProblemSetProblem
    {
        public virtual ProblemSet ProblemSet { get; set; }
        public int ProblemSetId { get; set; }
        public virtual Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public string ProblemSetProblemId { get; set; }
        // Concurrency danger zone
        // Number of submissions.
        public int Submissions { get; set; }
        // Number of Accepted submissions.
        public int Accepts { get; set; }
        // Fraction of accepts to submissions. Computed at client side.
        public decimal? AcceptsToSubmissions {
            get {
                return (Submissions == 0 ? (decimal?) null : (decimal) Accepts / (decimal) Submissions);
            }
            private set {

            }
        }
        public ICollection<ProblemSubmission> ProblemSubmissions { get; set; }
    }
}