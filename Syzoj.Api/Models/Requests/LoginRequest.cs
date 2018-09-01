using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Filters;

namespace Syzoj.Api.Models.Requests
{
    public class LoginRequest
    {
        /// <summary>
        /// Username of the user to login as.
        /// </summary>
        [UserName]
        public string UserName;
        /// <summary>
        /// Password of the user to login as.
        /// </summary>
        [MinLength(6)]
        public string Password;
    }
}