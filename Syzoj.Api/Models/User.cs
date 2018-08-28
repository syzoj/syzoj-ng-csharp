using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models.Attributes;

namespace Syzoj.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [UserName]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public UserPasswordType PasswordType { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
    }

    public enum UserPasswordType
    {
        Plain = 0,
        OldSyzoj = 1,
    }
}