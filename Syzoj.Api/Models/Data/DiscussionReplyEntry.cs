using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models.Data
{
    public class DiscussionReplyEntry
    {
        [Key]
        public int Id { get; set; }
        public virtual DiscussionEntry DiscussionEntry { get; set; }
        public int DiscussionEntryId { get; set; }
        public virtual User Author { get; set; }
        public int AuthorId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime? TimeCreated { get; set; }
    }
}