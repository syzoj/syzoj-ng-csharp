using MessagePack;

namespace Syzoj.Api.Models.Data
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyProblemData
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Example { get; set; }
        public string LimitAndHint { get; set; }
        public string[] Tags { get; set; }
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