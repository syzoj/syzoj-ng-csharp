using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;
using MessagePack;
using Syzoj.Api.Models;

namespace Syzoj.Api.Services
{
    [NonController]
    public class LegacyProblemController : ControllerBase, IProblemController, IProblemSubmissionController
    {
        private int controllerLevel = 0;
        public void SetFullController()
        {
            controllerLevel = 0;
        }
        public void SetSubmitonlyController()
        {
            controllerLevel = 1;
        }
        public void SetViewonlyController()
        {
            controllerLevel = 2;
        }
        public async Task<IActionResult> GetProblem(ProblemSetProblem psp, string action)
        {
            switch(action)
            {
                case null:
                    if(controllerLevel <= 2)
                    {
                        return Ok(new {
                            Status = "Success",
                            ProblemStatement = MessagePackSerializer.Deserialize<LegacySyzojStatement>(psp.Problem._Statement)
                        });
                    }
                    goto default;
                default:
                    return NotFound(new {
                        Status = "Fail",
                        Message = "Unsupported action"
                    });
            }
        }

        public Task<IActionResult> GetSubmission(ProblemSubmission submission, string action)
        {
            throw new System.NotImplementedException();
        }

        public Task<IActionResult> PostProblem(ProblemSetProblem psp, string action)
        {
            throw new System.NotImplementedException();
        }

        public Task<IActionResult> PostSubmission(ProblemSubmission submission, string action)
        {
            throw new System.NotImplementedException();
        }
    }
}