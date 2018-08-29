using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Models.Data
{
    public class DiscussionEntry
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public User Author { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public DateTime? TimeCreated { get; set; }
        [Required]
        public DateTime? TimeModified { get; set; }
        [Required]
        public DateTime? TimeLastReply { get; set; }
        public virtual ICollection<DiscussionReplyEntry> Replies { get; set; }
        public virtual ICollection<ForumDiscussion> Forums { get; set; }
    }
}