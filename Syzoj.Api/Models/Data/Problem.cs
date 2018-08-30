using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace Syzoj.Api.Models.Data
{
    public class Problem
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public virtual ICollection<ProblemSetProblem> ProblemSets { get; set; }
        public string Title { get; set; }
        public ProblemType Type { get; set; }
        [Column("Data")]
        public byte[] _Data { get; set; }
        public virtual ICollection<ProblemSubmission> ProblemSubmissions { get; set; }
    }

    public enum ProblemType
    {
        SyzojLegacyTraditional = 1,
        SyzojLegacySubmitAnswer = 2,
        SyzojLegacyInteraction = 3,
    }
}