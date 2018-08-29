using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models.Data
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }
        // For debugging purposes only
        public string Info { get; set; }
        public virtual ICollection<ForumDiscussion> DiscussionEntries { get; set; }
    }
}