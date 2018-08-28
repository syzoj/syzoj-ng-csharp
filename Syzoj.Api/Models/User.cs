using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public UserPasswordType PasswordType { get; set; }
        public string Password { get; set; }
    }

    public enum UserPasswordType
    {
        Plain = 0,
        OldSyzoj = 1,
    }
}