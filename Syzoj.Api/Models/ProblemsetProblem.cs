using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class ProblemsetProblem
    {
        public int ProblemsetId { get; set; }
        public Problemset Problemset { get; set; }
        public int ProblemId { get; set; }
        public Problem Problem { get; set; }
        [MinLength(1)]
        [MaxLength(128)]
        [Required]
        public string ProblemsetProblemId { get; set; }
    }
}