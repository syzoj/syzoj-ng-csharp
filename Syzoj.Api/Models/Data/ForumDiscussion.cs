using System;

namespace Syzoj.Api.Models.Data
{
    public class ForumDiscussion
    {
        public virtual Forum Forum { get; set; }
        public int ForumId { get; set; }
        public virtual DiscussionEntry DiscussionEntry { get; set; }
        public int DiscussionEntryId { get; set; }
        public DateTime TimeLastReply { get; set; } // used for sorting
    }
}