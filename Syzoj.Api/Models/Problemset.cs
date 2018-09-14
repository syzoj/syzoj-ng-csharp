using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class Problemset
    {
        [Key]
        public int Id { get; set; }
        public ICollection<ProblemsetProblem> ProblemsetProblems { get; set; }
    }
}