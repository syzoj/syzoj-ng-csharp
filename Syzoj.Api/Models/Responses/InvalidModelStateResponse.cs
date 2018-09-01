using System.Collections.Generic;

namespace Syzoj.Api.Models.Responses
{
    /// <summary>
    /// The request is invalid.
    /// </summary>
    public class InvalidModelStateResponse
    {
        public string Status {
            get {
                return "Fail";
            }
        }
        public int Code {
            get {
                return 1;
            }
        }
        public string Message {
            get {
                return "Invalid request detected.";
            }
        }
        public IEnumerable<ModelStateError> Errors { get; set; }
    }
}