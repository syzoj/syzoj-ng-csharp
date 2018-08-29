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
        // Number of submissions.
        public int Submissions { get; set; }
        // Number of Accepted submissions.
        public int Accepts { get; set; }
        public ProblemDataType DataType { get; set; }
        [Column("Data")]
        public byte[] _Data { get; set; }
        public T GetData<T>()
        {
            return MessagePackSerializer.Deserialize<T>(_Data);
        }
        public void SetData<T>(T Data)
        {
            _Data = MessagePackSerializer.Serialize(Data);
        }
    }

    public enum ProblemDataType
    {
        SyzojLegacyTraditional = 1,
        SyzojLegacySubmitAnswer = 2,
        SyzojLegacyInteraction = 3,
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyProblemData
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Example { get; set; }
        public string LimitAndHint { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyTraditionalProblemData : SyzojLegacyProblemData
    {
        public int TimeLimit { get; set; }
        public int MemoryLimit { get; set; }
        public bool FileIo { get; set; }
        public string FileIoInputName { get; set; }
        public string FileIoOutputName { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacySubmitAnswerProblemData : SyzojLegacyProblemData
    {
        
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyInteractionProblemData : SyzojLegacyProblemData
    {
        public int TimeLimit { get; set; }
        public int MemoryLimit { get; set; }
    }
}