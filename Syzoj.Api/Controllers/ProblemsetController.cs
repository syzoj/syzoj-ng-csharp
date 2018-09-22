using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syzoj.Api.Data;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using System;
using Z.EntityFramework.Plus;

namespace Syzoj.Api.Controllers
{
    [Route("api/problemset")]
    public class ProblemsetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ProblemParserProvider problemParserProvider;
        private readonly ProblemsetManagerProvider problemsetManagerProvider;
        public ProblemsetController(ApplicationDbContext dbContext, ProblemParserProvider problemParserProvider, ProblemsetManagerProvider problemsetManagerProvider)
        {
            this.dbContext = dbContext;
            this.problemParserProvider = problemParserProvider;
            this.problemsetManagerProvider = problemsetManagerProvider;
        }

        public class ProblemsetProblemListResponse : BaseResponse
        {
            /// <summary>
            /// The list of problems from the problemset.
            /// </summary>
            public IEnumerable<ProblemsetProblemSummaryEntry> Problems { get; set; }
        }
        public class ProblemsetProblemSummaryEntry
        {
            /// <summary>
            /// The global problem ID for the problem.
            /// </summary>
            public Guid ProblemId { get; set; }

            /// <summary>
            /// The problem identifier in this problemset.
            /// </summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>
            /// The title of the problem.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// The type of the problem.
            /// </summary>
            public string ProblemType { get; set; }
        }

        /// <summary>
        /// Gets the list of all problems in the problemset.
        /// </summary>
        /// <param name="Id">The ID of the problemset.</param>
        /// <param name="sort">Can be either `asc` or `desc`, specifying how the list shoud be sorted.</param>
        /// <param name="key">The sorting key.</param>
        /// <param name="page">Page number.</param>
        // TODO: Implement sorting and pagination
        [HttpGet("{Id}/problems")]
        public async Task<ActionResult<ProblemsetProblemListResponse>> GetProblemList([FromRoute] Guid Id, [FromQuery] string sort, [FromQuery] string key, [FromQuery] int page)
        {
            throw new NotImplementedException();
        }

        public class GetProblemResponse : BaseResponse
        {
            /// <summary>
            /// The global problem ID for the problem.
            /// </summary>
            public Guid ProblemId { get; set; }

            /// <summary>
            /// The problem identifier in this problemset.
            /// </summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>
            /// The problem's title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// The problem's type.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// The content of the problem. The exact model depends on the problem's type.
            /// </summary>
            public object Content { get; set; }

            /// <summary>
            /// Whether the current user can submit to the problem.
            /// </summary>
            public bool Submittable { get; set; }
        }

        /// <summary>
        /// Gets problem statement for a problem.
        /// </summary>
        [HttpGet("{Id}/problem/{pid}")]
        public async Task<ActionResult<GetProblemResponse>> GetProblem(Guid Id, string pid)
        {
            throw new NotImplementedException();
        }

        public class LoadProblemResponse : BaseResponse
        {
        }
        /// <summary>
        /// Loads the problem statement from data folder.
        /// </summary>
        [HttpPost("{Id}/problem/{pid}/load")]
        public async Task<ActionResult<LoadProblemResponse>> LoadProblem(Guid Id, string pid)
        {
            throw new NotImplementedException();
        }

        public class SubmitProblemRequest
        {
            /// <summary>
            /// The contents of the submission.
            /// </summary>
            public object Content { get; set; }
        }
        public class SubmitProblemResponse : BaseResponse
        {
            /// <summary>
            /// The id of the new submission.
            /// </summary>
            public Guid SubmissionId { get; set; }
        }
        /// <summary>
        /// Creates a new submission.
        /// </summary>
        [HttpPost("{Id}/problem/{pid}/submit")]
        public async Task<ActionResult<SubmitProblemResponse>> SubmitProblem([FromRoute] Guid Id, [FromRoute] string pid, [FromBody] SubmitProblemRequest request)
        {
            throw new NotImplementedException();
        }

        public class GetSubmissionsResponse : BaseResponse
        {
            /// <summary>
            /// The list of submissions.
            /// </summary>
            public IEnumerable<SubmissionSummaryEntry> Submissions { get; set; }
        }
        public class SubmissionSummaryEntry
        {
            /// <summary>The global id of the corresponding problem.</summary>
            public Guid ProblemId { get; set; }

            /// <summary>The ID of the corresponding problem.</summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>The ID of the submission.</summary>
            public Guid SubmissionId { get; set; }

            /// <summary>
            /// Problem Type specific object describing the submission.
            /// </summary>
            public object Content { get; set; }
        }
        /// <summary>
        /// Gets all submissions in the problemset.
        /// </summary>
        [HttpGet("{Id}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetSubmissions(Guid Id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all submissions for a specific problem in the problemset.
        /// </summary>
        [HttpGet("{Id}/problem/{pid}/submissions")]
        public async Task<ActionResult<GetSubmissionsResponse>> GetProblemSubmissions(Guid Id, string pid)
        {
            throw new NotImplementedException();
        }

        public class GetSubmissionResponse : BaseResponse
        {
            /// <summary>The global id of the corresponding problem.</summary>
            public Guid ProblemId { get; set; }

            /// <summary>The ID of the corresponding problem.</summary>
            public string ProblemsetProblemId { get; set; }

            /// <summary>The ID of the submission.</summary>
            public Guid SubmissionId { get; set; }

            /// <summary>
            /// Problem Type specific object describing the submission.
            /// </summary>
            public object Content { get; set; }
        }
        /// <summary>
        /// Gets a specified submission.
        /// </summary>
        [HttpGet("{Id}/submission/{sid}")]
        public async Task<ActionResult<GetSubmissionResponse>> GetSubmission(Guid Id, Guid sid)
        {
            throw new NotImplementedException();
        }
    }
}