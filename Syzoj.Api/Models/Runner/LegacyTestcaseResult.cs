using MessagePack;
namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyTestcaseResult
    {
        public LegacyTaskStatus status { get; set; }
        public LegacyTestcaseDetails result { get; set; }
        public string errorMessage { get; set; }
    }
}