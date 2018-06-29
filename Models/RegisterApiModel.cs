using System;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Models
{
    [Serializable]
    public class RegisterApiModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}