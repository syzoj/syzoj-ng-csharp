using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardTestData : IValidatableObject
    {
        [Required]
        public IList<StandardTestCase> TestCases { get; set; } = new StandardTestCase[0];
        [Required]
        public IList<StandardJudgerSettings> JudgerSettings { get; set; } = new StandardJudgerSettings[0];
        [Required]
        public IList<Subtask> Subtasks { get; set; } = new Subtask[0];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            for(var i = 0; i < TestCases.Count; ++i)
            {
                var testCase = TestCases[i];
                if(testCase.JudgerSettings >= JudgerSettings.Count || testCase.JudgerSettings < 0)
                {
                    yield return new ValidationResult("Invalid JudgerSettings index", new [] { $"TestCases[{i}].JudgerSettings" });
                }
            }

            for(var i = 0; i < Subtasks.Count; ++i)
            {
                var subtask = Subtasks[i];
                for(var j = 0; j < subtask.TestCases.Count; ++j)
                {
                    var k = subtask.TestCases[j];
                    if(k >= TestCases.Count || k < 0)
                    {
                        yield return new ValidationResult("Invalid TestCase index", new [] { $"Subtasks[{i}].TestCases[{j}]" });
                    }
                }
                for(var j = 0; j < subtask.Dependencies.Count; ++j)
                {
                    var k = subtask.Dependencies[j];
                    if(k >= j || k < 0)
                    {
                        yield return new ValidationResult("Dependency must be before current subtask", new [] { $"Subtasks[{i}].Dependencies[{k}]"});
                    }
                }
            }
        }
    }
}