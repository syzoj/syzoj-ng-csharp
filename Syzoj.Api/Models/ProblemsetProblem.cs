using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class ProblemsetProblem
    {
        public int ProblemsetId { get; set; }
        public virtual Problemset Problemset { get; set; }
        public int ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
        [MinLength(1)]
        [MaxLength(128)]
        [Required]
        public string ProblemsetProblemId { get; set; }
    }
}