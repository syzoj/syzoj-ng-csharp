using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class Problemset
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Type { get; set; }
        public virtual ICollection<ProblemsetProblem> ProblemsetProblems { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}