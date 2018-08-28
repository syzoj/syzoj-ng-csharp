namespace Syzoj.Api.Models.Requests
{
    public class CreateReplyRequest
    {
        public string SessionId { get; set; }
        public int DiscussionEntryId { get; set; }
        public string Content { get; set; }
    }
}