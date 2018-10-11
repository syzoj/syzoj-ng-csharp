using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    /// <summary>
    /// The statement of a problem.
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public class ProblemStatement
    {
        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; } = "";
        [Required(AllowEmptyStrings = true)]
        public string InputFormat { get; set; } = "";
        [Required(AllowEmptyStrings = true)]
        public string OutputFormat { get; set; } = "";
        [Required(AllowEmptyStrings = true)]
        public string Example { get; set; } = "";
        [Required(AllowEmptyStrings = true)]
        public string LimitAndHint { get; set; } = "";
    }
}