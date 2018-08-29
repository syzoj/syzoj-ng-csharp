using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using MessagePack;
using Syzoj.Api.Models;
using Syzoj.Api.Utils;
using System;

namespace Syzoj.Api.Services
{
    public class SessionMiddleware : IMiddleware
    {
        private readonly IConnectionMultiplexer redisConnection;
        public SessionMiddleware(IConnectionMultiplexer redisConnection)
        {
            this.redisConnection = redisConnection;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string sessionId = context.Request.Headers["Session"];
            bool hasSession = false;
            if(sessionId != null)
            {
                string redisKeyName = "syzoj:session:" + sessionId;
                byte[] data = await redisConnection.GetDatabase().StringGetAsync(redisKeyName);
                if(data != null)
                {
                    context.Items["Session"] = MessagePackSerializer.Deserialize<Session>(data);
                    hasSession = true;
                }
                else
                {
                    sessionId = null;
                }
            }
            if(sessionId == null)
            {
                sessionId = MiscUtils.ConvertToHex(MiscUtils.GetRandomBytes(16));
                Session defaultSession = new Session();
                context.Items["Session"] = defaultSession;
            }
            context.Response.Headers["Session"] = sessionId;
            await next(context);
            if(context.Items["Session"] is Session sess)
            {
                string redisKeyName = "syzoj:session:" + sessionId;
                byte[] data = MessagePackSerializer.Serialize<Session>(sess);
                await redisConnection.GetDatabase().StringSetAsync(
                    redisKeyName,
                    data,
                    sess.Expiration
                );
            }
            else if(hasSession)
            {
                string redisKeyName = "syzoj:session:" + sessionId;
                await redisConnection.GetDatabase().KeyDeleteAsync(
                    redisKeyName
                );
            }
        }
    }
}