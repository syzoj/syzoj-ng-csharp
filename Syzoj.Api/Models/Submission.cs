using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syzoj.Api.Models
{
    public class Submission
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProblemsetId { get; set; }
        public virtual Problemset Problemset { get; set; }
        public Guid ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual ProblemsetProblem ProblemsetProblem { get; set; }
        [Column(TypeName = "jsonb")]
        public string Content { get; set; }
        public byte[] Data { get; set; }
    }
}