using MessagePack;
namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyCompilationResult
    {
        public LegacyTaskStatus status { get; set; }
        public string message { get; set; }
    }
}