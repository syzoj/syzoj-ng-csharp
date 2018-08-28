using MessagePack;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Session
    {
        [IgnoreMember]
        public string SessionID;
        public int? UserId { get; set; }
        public string UserName { get; set;}
        // TODO: Include device metadata such as User-Agent and IP address

        public void SetUser(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
        }
    }
}