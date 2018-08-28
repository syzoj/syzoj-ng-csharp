using System;
using System.Threading.Tasks;
using MessagePack;
using StackExchange.Redis;
using Syzoj.Api.Models;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Services
{
    public class SessionManager : ISessionManager
    {
        private readonly IConnectionMultiplexer redisConnection;
        public SessionManager(IConnectionMultiplexer redisConnection)
        {
            this.redisConnection = redisConnection;
        }

        public Task<string> CreateSession(Session sess)
        {
            string sessionId = MiscUtils.ConvertToHex(MiscUtils.GetRandomBytes(16));
            string keyName = String.Format("syzoj:session:{0}", sessionId);
            IDatabase db = redisConnection.GetDatabase();
            db.StringSet(keyName, MessagePackSerializer.Serialize(sess), TimeSpan.FromDays(30), flags: CommandFlags.FireAndForget);
            return Task.FromResult(sessionId);
        }

        public async Task<Session> GetSession(string sessionID)
        {
            string keyName = string.Format("syzoj:session:{0}", sessionID);
            IDatabase db = redisConnection.GetDatabase();
            byte[] data = await db.StringGetAsync(keyName);
            if(data == null)
                return null;
            return MessagePackSerializer.Deserialize<Session>(data);
        }
    }
}