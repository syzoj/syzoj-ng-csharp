namespace Syzoj.Api.Models.Responses
{
    public class DiscussionEntryCreatedResponse : BaseResponse
    {
        public DiscussionEntryCreatedResponse()
        {
            Status = "Success";
            Code = 0;
        }
        /// <summary>
        /// The id of the discussion entry created.
        /// </summary>
        public int DiscussionEntryId { get; set; }
    }
}