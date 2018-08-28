using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models.Attributes;
using Syzoj.Api.Utils;

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
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public void SetPassword(string password)
        {
            var (salt, hash) = HashUtils.GenerateHashedPassword(password);
            this.PasswordHash = hash;
            this.PasswordSalt = salt;
            this.PasswordType = UserPasswordType.SaltHashed;
        }

        public bool CheckPassword(string password)
        {
            return HashUtils.VerifyHash(PasswordSalt, PasswordHash, password);
        }
    }

    public enum UserPasswordType
    {
        Plain = 0,
        SaltHashed = 1,
        OldSyzojStyle = 2,
    }
}