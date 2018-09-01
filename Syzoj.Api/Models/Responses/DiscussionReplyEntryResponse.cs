using System;

namespace Syzoj.Api.Models.Responses
{
    public class DiscussionReplyEntryResponse
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime? TimeCreated { get; set; }
        public string Content { get; set; }
    }
}