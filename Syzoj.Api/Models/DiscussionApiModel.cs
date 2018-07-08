namespace Syzoj.Api.Models
{
    public class DiscussionApiModel
    {
        public int Id { get; set; }

        public string Content { get; set; }
        
        public bool? ShowInBoard { get; set; }
    }

    public class DiscussionCreateApiModel
    {
        public string Content { get; set; }
        
        public bool? ShowInBoard { get; set; }
    }
}