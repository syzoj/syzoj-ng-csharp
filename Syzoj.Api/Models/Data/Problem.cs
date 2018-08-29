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
        // Number of submissions.
        public int Submissions { get; set; }
        // Number of Accepted submissions.
        public int Accepts { get; set; }
        [Column("Data")]
        public byte[] _Data { get; set; }
        [NotMapped]
        public ProblemData Data
        {
            get { return MessagePackSerializer.Deserialize<ProblemData>(_Data); }
            set { _Data = MessagePackSerializer.Serialize(_Data); }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ProblemData
    {
        public ProblemDataType Type { get; set; }
        public SyzojLegacyStandardProblemData SyzojLegacyStandardProblemData { get; set; }
        public SyzojLegacyAnswerSubmissionProblemData SyzojLegacyAnswerSubmissionProblemData { get; set; }
        public SyzojLegacyInteractiveProblemData SyzojLegacyInteractiveProblemData { get; set; }
    }

    public enum ProblemDataType
    {
        SyzojLegacyStandard = 1,
        SyzojLegacyAnswerSubmission = 2,
        SyzojLegacyInteractive = 3,
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyProblemData
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Example { get; set; }
        public string HintAndLimit { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyStandardProblemData : SyzojLegacyProblemData
    {
        public int TimeLimit { get; set; }
        public int MemoryLimit { get; set; }
        public bool FileIo { get; set; }
        public string FileIoInputName { get; set; }
        public string FileIoOutputName { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyAnswerSubmissionProblemData : SyzojLegacyProblemData
    {
        
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyInteractiveProblemData : SyzojLegacyProblemData
    {
        public int TimeLimit { get; set; }
        public int MemoryLimit { get; set; }
    }
}