using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardProblemContent
    {
        public ProblemStatement Statement { get; set; }
        public StandardTestData TestData { get; set; }
        public string[] Tags { get; set; }
    }
}