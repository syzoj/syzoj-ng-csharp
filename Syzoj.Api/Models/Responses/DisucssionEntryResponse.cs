using System;
using System.Collections.Generic;

namespace Syzoj.Api.Models.Responses
{
    public class DiscussionEntryResponse
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime? TimeCreated { get; set; }
        public DateTime? TimeLastReply { get; set; }
        public DateTime? TimeModified { get; set; }
        public IEnumerable<DiscussionReplyEntryResponse> Replies;
    }
}