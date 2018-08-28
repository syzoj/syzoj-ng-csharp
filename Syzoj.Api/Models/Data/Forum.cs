using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models.Data
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<DiscussionEntry> DiscussionEntries { get; set; }
    }
}