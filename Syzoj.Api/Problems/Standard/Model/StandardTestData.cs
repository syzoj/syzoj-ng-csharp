using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardTestData
    {
        [Required]
        public IDictionary<Guid, StandardTestCase> TestCases { get; set; } = new Dictionary<Guid, StandardTestCase>();
        [Required]
        public IDictionary<Guid, StandardJudgerSettings> JudgerSettings { get; set; } = new Dictionary<Guid, StandardJudgerSettings>();
        [Required]
        public IList<Subtask> Subtasks { get; set; } = new List<Subtask>();
    }
}