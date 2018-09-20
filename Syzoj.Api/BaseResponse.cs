namespace Syzoj.Api
{
    /// <summary>
    /// The base class for all responses. An error means an unexpected situation that
    /// can not be detected from the client.
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// Whether the request was successful or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// An error message if the request failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The arguments for the error message.
        /// </summary>
        public string[] ErrorArguments { get; set; }
    }
}