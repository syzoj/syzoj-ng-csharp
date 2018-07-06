using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class DiscussionEntry
    {
        [Key]
        public int Id { get; set; }

        public User Author { get; set; }

        public string Content { get; set; }
        
        public ReplyEntry[] Reply { get; set; }
    }

    public class ReplyEntry
    {
        [Key]
        public string Id { get; set; }
        
        public string Content { get; set; }
        
        public User Author { get; set; }
    }
}