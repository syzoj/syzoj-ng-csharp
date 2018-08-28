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

        // Inserts/updates a new session into backend and populates SessionID attribute
        public Task UpdateSession(Session sess)
        {
            if(sess.SessionID == null)
                sess.SessionID = MiscUtils.ConvertToHex(MiscUtils.GetRandomBytes(16));
            string keyName = String.Format("syzoj:session:{0}", sess.SessionID);
            IDatabase db = redisConnection.GetDatabase();
            return db.StringSetAsync(keyName, MessagePackSerializer.Serialize(sess), TimeSpan.FromDays(30));
        }

        public async Task<Session> GetSession(string sessionID)
        {
            string keyName = string.Format("syzoj:session:{0}", sessionID);
            IDatabase db = redisConnection.GetDatabase();
            byte[] data = await db.StringGetAsync(keyName);
            if(data == null)
                return null;
            Session sess = MessagePackSerializer.Deserialize<Session>(data);
            sess.SessionID = sessionID;
            return sess;
        }
    }
}