using System.Collections.Generic;

namespace Syzoj.Api.Mvc
{
    /// <summary>
    /// The object response class for all responses from REST APIs.
    /// </summary>
    public class CustomResponse<TResult>
    {
        public bool Success { get; set; } = true;
        public IEnumerable<ActionError> Errors { get; set; }
        public TResult Result { get; set; }

        public CustomResponse() { }
        public CustomResponse(TResult result)
        {
            this.Result = result;
        }
    }

    public class CustomResponse
    {
        public bool Success { get; set; } = true;
        public IEnumerable<ActionError> Errors { get; set; }
    }
}