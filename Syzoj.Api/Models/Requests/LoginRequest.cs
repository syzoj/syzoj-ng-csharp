using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Models.Attributes;

namespace Syzoj.Api.Models.Requests
{
    public class LoginRequest
    {
        [UserName]
        public string UserName;
        [MinLength(6)]
        public string Password;
    }
}