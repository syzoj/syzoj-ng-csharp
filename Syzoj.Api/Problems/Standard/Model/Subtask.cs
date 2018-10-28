using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using Syzoj.Api.Mvc;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Subtask
    {
        [Required]
        public IList<int> Dependencies { get; set; } = new int[0];
        [Required]
        public IList<int> TestCases { get; set; } = new int[0];
        public SubtaskScoringStrategy Strategy { get; set; }
        [NonNegative]
        public decimal Score { get; set; }
    }

    public enum SubtaskScoringStrategy
    {
        Sum = 1,
        Min = 2,
    }
}