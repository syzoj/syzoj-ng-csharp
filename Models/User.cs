using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    [Serializable]
    public class User
    {
        public string Name { get; set; }

        [Key]
        public string Email { get; set; }

        public string HashedPassword { get; set; }
        
        public string PasswordSalt { get; set; }
    }
}