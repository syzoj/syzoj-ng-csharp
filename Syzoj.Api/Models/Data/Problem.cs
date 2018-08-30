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
        [Column("Statement")]
        public byte[] _Statement { get; set; }
        public ProblemType Type { get; set; }
        [Column("Data")]
        public byte[] _Data { get; set; }
        public virtual ICollection<ProblemSubmission> ProblemSubmissions { get; set; }

        [NotMapped]
        public ProblemStatement Statement {
            get {
                return MessagePackSerializer.Deserialize<ProblemStatement>(_Statement);
            }
            set {
                _Statement = MessagePackSerializer.Serialize<ProblemStatement>(value);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ProblemStatement
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Example { get; set; }
        public string LimitAndHint { get; set; }
        public string[] Tags { get; set; }
    }

    public enum ProblemType
    {
        SyzojLegacyTraditional = 1,
        SyzojLegacySubmitAnswer = 2,
        SyzojLegacyInteraction = 3,
    }
}