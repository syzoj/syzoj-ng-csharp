using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardProblemContent
    {
        [Required]
        public ProblemStatement Statement { get; set; }
        [Required]
        public StandardTestData TestData { get; set; }
        public string[] Tags { get; set; }
    }
}