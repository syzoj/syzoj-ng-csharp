using MessagePack;

namespace Syzoj.Api.Models.Data
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyTraditionalProblemData
    {
        public int TimeLimit { get; set; }
        public int MemoryLimit { get; set; }
        public bool FileIo { get; set; }
        public string FileIoInputName { get; set; }
        public string FileIoOutputName { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacySubmitAnswerProblemData
    {
        
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyInteractionProblemData
    {
        public int TimeLimit { get; set; }
        public int MemoryLimit { get; set; }
    }
}