namespace Syzoj.Api.Problems.Interfaces
{
    /// <summary>
    /// Defines the model used by the frontend to produce problem view.
    /// </summary>
    public class ProblemViewModel
    {
        /// <summary>
        /// The type name of the problem. A corresponding type name should be
        /// registered at frontend.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The data of the problem, handled at frontend.
        /// </summary>
        public object Data { get; set; }
    }
}