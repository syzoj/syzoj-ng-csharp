using System;
using System.Collections.Generic;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Subtask
    {
        public IEnumerable<int> Dependencies { get; set; }
        public IEnumerable<Guid> TestCases { get; set; }
        public SubtaskScoringStrategy Strategy { get; set; }
        public decimal Score { get; set; }
    }

    public enum SubtaskScoringStrategy
    {
        Sum = 1,
        Min = 2,
    }
}