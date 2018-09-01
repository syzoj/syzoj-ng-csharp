using System.Collections.Generic;

namespace Syzoj.Api.Models.Responses
{
    public class DiscussionEntryListResponse : BaseResponse
    {
        public IEnumerable<DiscussionEntryResponse> DiscussionEntries;
        public DiscussionEntryListResponse()
        {
            Status = "Success";
            Code = 0;
        }
    }
}