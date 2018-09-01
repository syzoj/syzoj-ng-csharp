namespace Syzoj.Api.Models.Responses
{
    public class DiscussionReplyEntryCreatedResponse : BaseResponse
    {
        public int DiscussionReplyEntryId { get; set; }
        public DiscussionReplyEntryCreatedResponse()
        {
            Status = "Success";
            Code = 0;
        }
    }
}