using System.Collections.Generic;

namespace Syzoj.Api.Models.Responses
{
    /// <summary>
    /// Response indicatting that the request is invalid.
    /// </summary>
    public class InvalidModelStateResponse : BaseResponse
    {
        public InvalidModelStateResponse(): base()
        {
            Status = "Fail";
            Code = 1;
            Message = "Invalid request detected";
        }

        /// <summary>
        /// An array of errors detected in request.
        /// </summary>
        public IEnumerable<ModelStateError> Errors { get; set; }
    }
}