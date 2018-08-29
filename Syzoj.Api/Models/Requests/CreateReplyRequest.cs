namespace Syzoj.Api.Models.Requests
{
    public class CreateReplyRequest
    {
        public int DiscussionEntryId { get; set; }
        public string Content { get; set; }
    }
}