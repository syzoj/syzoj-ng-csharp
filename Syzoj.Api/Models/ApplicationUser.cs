using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Syzoj.Api.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public virtual ICollection<LoginSession> Sessions { get; set; }
        
        public DateTime RegisteredOn { get; set; }
    }
}