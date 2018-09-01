namespace Syzoj.Api.Models.Responses
{
    public class ModelStateError
    {
        /// <summary>
        /// The key of the part of request that is invalid.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The error message explaning how the request is invalid.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}