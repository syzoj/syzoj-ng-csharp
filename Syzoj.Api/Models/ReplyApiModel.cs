namespace Syzoj.Api.Models
{
    public class ReplyApiModel
    {
        public int DiscussionId { get; set; }

        public string Content { get; set; }
    }
    
    public class SubReplyApiModel
    {
        
        public int ReplyId { get; set; }

        public string Content { get; set; }
    }
    
    public class ReplyChangeApiModel
    {
        
        public int Id { get; set; }

        public string Content { get; set; }
    }
}