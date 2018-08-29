using System;
using MessagePack;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Session
    {
        public int? UserId { get; set; }
        public string UserName { get; set;}
        public TimeSpan Expiration { get; set; }
        public Session()
        {
            Expiration = TimeSpan.FromMinutes(20);
        }
        // TODO: Include device metadata such as User-Agent and IP address

        public void SetUser(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
        }
    }
}