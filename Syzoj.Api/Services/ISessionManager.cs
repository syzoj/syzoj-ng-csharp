using System.Threading.Tasks;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface ISessionManager
    {
        bool IsAuthenticated();
        // Call after checking IsAuthenticated()
        int GetAuthenticatedUserId();
        string GetAuthenticatedUserName();
        Task AuthenticateUserAsync(User user);
        Task UnauthenticateUserAsync();
    }
}