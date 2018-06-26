using System;

namespace Syzoj.Api.Models
{
    /// 表示一个会话。应该通过序列化的方式放入 Redis 缓存中。
    [Serializable]
    public class Session
    {
        public string Id { get; set; }

        public string Device { get; set; }

        public User User { get; set; }

        public DateTime TimeOut { get; set; }
    }
}