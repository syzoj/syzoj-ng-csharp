using System;

namespace Syzoj.Api.Models
{
    [Serializable]
    public class User
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }
    }
}