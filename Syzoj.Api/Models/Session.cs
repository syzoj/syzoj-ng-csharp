using MessagePack;

namespace Syzoj.Api.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Session
    {
        public int UserId { get; set; }
        // TODO: Device-specific attributes like User Agent, IP, etc.
    }
}