using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Filters;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Models.Data
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [UserName]
        [Column(TypeName = "CITEXT")]
        public string UserName { get; set; }
        /// <remark>
        /// Email address are difficult to handle; they indeed ARE case sensitive
        /// in the local part but not in the domain name part. However most email
        /// providers don't distinguish cases. So we just don't distinguish cases
        /// at all here.
        /// </remark>
        [EmailAddress]
        [Column(TypeName = "CITEXT")]
        public string Email { get; set; }
        public UserPasswordType PasswordType { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public virtual ICollection<ProblemSubmission> ProblemSubmissions { get; set; }

        public void SetPassword(string password)
        {
            var (salt, hash) = HashUtils.GenerateHashedPassword(password);
            this.PasswordHash = hash;
            this.PasswordSalt = salt;
            this.PasswordType = UserPasswordType.SaltHashed;
        }

        public bool CheckPassword(string password)
        {
            switch(PasswordType)
            {
                case UserPasswordType.Plain:
                    return HashUtils.ConstantTimeCompare(System.Text.Encoding.ASCII.GetBytes(password), PasswordHash);
                case UserPasswordType.OldSyzojStyle:
                    using (var md5 = MD5.Create())
                    {
                        byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password + "syzoj2_xxx"));
                        return HashUtils.ConstantTimeCompare(System.Text.Encoding.ASCII.GetBytes(MiscUtils.ConvertToHex(hashBytes)), PasswordHash);
                    };
                case UserPasswordType.SaltHashed:
                    return HashUtils.VerifyHash(PasswordSalt, PasswordHash, password);
                default:
                    return false;
            }
        }
    }

    public enum UserPasswordType
    {
        Plain = 0,
        OldSyzojStyle = 1,
        SaltHashed = 100,
    }
}