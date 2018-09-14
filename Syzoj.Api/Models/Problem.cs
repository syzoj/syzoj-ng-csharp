using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class Problem
    {
        [Key]
        public int Id { get; set; }
        public string ProblemType { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public byte[] Content { get; set; }
        public ICollection<ProblemsetProblem> ProblemsetProblems { get; set; }
    }
}