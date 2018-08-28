namespace Syzoj.Api.Models.Requests
{
    public class CreateDiscussionRequest
    {
        public string SessionID { get; set; }
        public int ForumId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}