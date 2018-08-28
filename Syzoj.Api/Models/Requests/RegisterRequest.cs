using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Models.Attributes;

namespace Syzoj.Api.Models.Requests
{
    public class RegisterRequest
    {
        [UserName]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
    }
}