using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models.Data
{
    public class ProblemSet
    {
        [Key]
        public int Id { get; set; }
        public string Info { get; set; }
        public ProblemSetType Type { get; set; }
        public virtual ICollection<ProblemSetProblem> ProblemSetProblem { get; set; }
        public virtual ICollection<ProblemSubmission> ProblemSubmissions { get; set; }
    }

    public enum ProblemSetType
    {
        Default = 1,
    }
}