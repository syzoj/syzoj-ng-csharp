using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardProblemContent
    {
        [Required]
        public ProblemStatement Statement { get; set; } = new ProblemStatement();
        [Required]
        public StandardTestData TestData { get; set; } = new StandardTestData();
        [Required]
        public IList<string> Tags { get; set; } = new List<string>();
    }
}