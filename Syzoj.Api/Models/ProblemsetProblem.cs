using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class ProblemsetProblem
    {
        public Guid ProblemsetId { get; set; }
        public virtual Problemset Problemset { get; set; }
        public Guid ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual ICollection<ProblemsetSubmission> Submissions { get; set; }
        [MinLength(1)]
        [MaxLength(128)]
        [Required]
        public string ProblemsetProblemId { get; set; }
        public string Title { get; set; }
    }
}