using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Models
{
    public class User
    {
        [Key]
        public int Id;
        [EmailAddress]
        public string Email;
        public UserPasswordType PasswordType;
        public string Password;
    }

    public enum UserPasswordType
    {
        Plain = 0,
        OldSyzoj = 1,
    }
}