using MessagePack;
namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyOverallResult
    {
        public LegacyErrorType? error { get; set; }
        public string systemMessage;
        public LegacyCompilationResult compile;
        public LegacyJudgeResult judge;
    }
}