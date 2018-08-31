using MessagePack;

namespace Syzoj.Api.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacySyzojStatement
    {
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string Examples { get; set; }
        public string LimitAndHint { get; set; }
        public string[] Tags { get; set; }
    }
}