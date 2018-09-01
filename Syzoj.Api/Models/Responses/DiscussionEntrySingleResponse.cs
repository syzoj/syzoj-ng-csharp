namespace Syzoj.Api.Models.Responses
{
    public class DiscussionEntrySingleResponse : BaseResponse
    {
        public DiscussionEntryResponse DiscussionEntry;
        public DiscussionEntrySingleResponse()
        {
            Status = "Success";
            Code = 0;
        }
    }
}