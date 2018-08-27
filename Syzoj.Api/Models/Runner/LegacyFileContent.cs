using MessagePack;
namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyFileContent
    {
        public string content { get; set; }
        public string name { get; set; }
    }
}