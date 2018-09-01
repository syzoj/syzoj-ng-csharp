using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models.Responses
{
    public class BaseResponse
    {
        /// <summary>
        /// Either `Success` or `Fail`, indicating whether the request was successful.
        /// </summary>
        /// <example>Success</example>
        public string Status { get; set; }
        
        /// <summary>
        /// The code of the response. `0` means success, `1` means invalid request,
        /// `2` means login required, and `3` means resource not found.
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// The error message. Only present when there's an error.
        /// </summary>
        public string Message { get; set; }
    }
}