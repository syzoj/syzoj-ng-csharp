using MessagePack;

namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyTestcaseDetails
    {
        public LegacyTestcaseResultType type { get; set; }
        public float time { get; set; }
        public float memory { get; set; }
        public LegacyFileContent input { get; set; }
        public LegacyFileContent output { get; set; }
        public float scoringRate { get; set; }
        public string userOutput { get; set; }
        public string userError { get; set; }
        public string spjMessage { get; set; }
        public string systemMessage { get; set; }
    }
}