using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    /// <summary>
    /// The statement of a problem.
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public class ProblemStatement
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Example { get; set; }
        public string LimitAndHint { get; set; }
    }
}