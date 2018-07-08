using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class LoginSession
    {
        [Key]
        public string Id { get; set; }
        
        public string Token { get; set; }

        /// <summary>
        /// Login identifiers, such as IP address, User-Agent, etc.
        /// </summary>
        public string Identifiers { get; set; }

        public virtual ApplicationUser User { get; set; }

        public DateTime LoginTime { get; set; }
        
        public SessionStatus Status { get; set; }
    }

    public enum SessionStatus
    {
        Active,
        Expired,
        Deactivated
    }
}