using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Subtask
    {
        [Required]
        public IEnumerable<int> Dependencies { get; set; } = new int[] {};
        [Required]
        public IEnumerable<Guid> TestCases { get; set; } = new Guid[] {};
        public SubtaskScoringStrategy Strategy { get; set; }
        public decimal Score { get; set; }
    }

    public enum SubtaskScoringStrategy
    {
        Sum = 1,
        Min = 2,
    }
}