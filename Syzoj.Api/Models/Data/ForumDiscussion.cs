namespace Syzoj.Api.Models.Data
{
    public class ForumDiscussion
    {
        public Forum Forum { get; set; }
        public int ForumId { get; set; }
        public DiscussionEntry DiscussionEntry { get; set; }
        public int DiscussionEntryId { get; set; }
    }
}