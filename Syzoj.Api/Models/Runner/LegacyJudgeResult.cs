using MessagePack;
namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyJudgeResult
    {
        public LegacySubtaskResult[] subtasks { get; set; }
    }
}