using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class DiscussionEntry
    {
        [Key]
        public int Id { get; set; }

        public virtual User Author { get; set; }

        public string Content { get; set; }
        
        public virtual ICollection<ReplyEntry> Replies { get; set; }
        
        public bool ShowInBoard { get; set; }
    }

    public class ReplyEntry
    {
        [Key]
        public int Id { get; set; }
        
        public string Content { get; set; }
        
        public virtual User Author { get; set; }
        
        public virtual ICollection<ReplyEntry> Replies { get; set; }
    }
}