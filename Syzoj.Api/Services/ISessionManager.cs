using System.Threading.Tasks;
using Syzoj.Api.Models;

namespace Syzoj.Api.Services
{
    public interface ISessionManager
    {
        Task<Session> GetSession(string SessionID);
        Task<string> CreateSession(Session sess);
    }
}