using MessagePack;
namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacySubtaskResult
    {
        public float? score { get; set; }
        public LegacyTestcaseResult[] cases { get; set; }
        // Message included status?
    }
}