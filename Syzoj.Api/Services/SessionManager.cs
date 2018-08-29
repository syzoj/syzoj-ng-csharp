using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Services
{
    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public bool IsAuthenticated()
        {
            Session sess = (Session) httpContextAccessor.HttpContext.Items["Session"];
            return sess.UserId != null;
        }
        public int GetAuthenticatedUserId()
        {
            Session sess = (Session) httpContextAccessor.HttpContext.Items["Session"];
            return (int) sess.UserId;
        }
        public string GetAuthenticatedUserName()
        {
            Session sess = (Session) httpContextAccessor.HttpContext.Items["Session"];
            return sess.UserName;
        }
        public Task AuthenticateUserAsync(User user)
        {
            Session sess = (Session) httpContextAccessor.HttpContext.Items["Session"];
            sess.UserId = user.Id;
            sess.UserName = user.UserName;
            sess.Expiration = TimeSpan.FromDays(30);
            return Task.CompletedTask;
        }
        public Task UnauthenticateUserAsync()
        {
            Session sess = (Session) httpContextAccessor.HttpContext.Items["Session"];
            sess.UserId = null;
            sess.UserName = null;
            sess.Expiration = TimeSpan.FromMinutes(20);
            return Task.CompletedTask;
        }
    }
}