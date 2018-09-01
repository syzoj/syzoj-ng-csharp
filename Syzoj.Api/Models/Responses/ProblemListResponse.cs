using System.Collections.Generic;

namespace Syzoj.Api.Models.Responses
{
    public class ProblemListResponse : BaseResponse
    {
        /// <summary>
        /// The list of problems in problemset.
        /// </summary>
        public IEnumerable<ProblemListEntryResponse> ProblemSet { get; set; }

        public ProblemListResponse()
        {
            Status = "Success";
            Code = 0;
        }
    }
}