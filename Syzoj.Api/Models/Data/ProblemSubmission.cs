using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syzoj.Api.Models.Data
{
    public class ProblemSubmission
    {
        [Key]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual ProblemSet ProblemSet { get; set; }
        public int ProblemSetId { get; set; }
        public virtual Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public virtual ProblemSetProblem ProblemSetProblem { get; set; }
        public ProblemSubmissionType Type { get; set; }
        [Column("SubmissionData")]
        public byte[] _SubmissionData { get; set; }
    }

    public enum ProblemSubmissionType
    {
    }
}