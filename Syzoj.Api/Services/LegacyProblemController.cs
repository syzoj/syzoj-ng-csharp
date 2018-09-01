using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models.Data;
using MessagePack;
using Syzoj.Api.Models;
using Syzoj.Api.Requests;
using Syzoj.Api.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;

namespace Syzoj.Api.Services
{
    [NonController]
    public class LegacyProblemController : ControllerBase, IProblemController, IProblemSubmissionController
    {
        private readonly ApplicationDbContext dbContext;
        public LegacyProblemController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private int controllerLevel = 100;
        public void SetFullController()
        {
            if(HttpContext.RequestServices.GetService<IHostingEnvironment>().IsDevelopment())
            {
                controllerLevel = 0;
                return;
            }
            controllerLevel = 0;
        }
        public void SetSubmitonlyController()
        {
            if(HttpContext.RequestServices.GetService<IHostingEnvironment>().IsDevelopment())
            {
                controllerLevel = 0;
                return;
            }
            controllerLevel = 1;
        }
        public void SetViewonlyController()
        {
            if(HttpContext.RequestServices.GetService<IHostingEnvironment>().IsDevelopment())
            {
                controllerLevel = 0;
                return;
            }
            controllerLevel = 2;
        }
        public async Task<IActionResult> Problem(ProblemSetProblem psp, string action)
        {
            switch(HttpContext.Request.Method)
            {
                case "GET":
                    switch(action)
                    {
                        case null:
                            if(controllerLevel <= 2)
                            {
                                return Ok(new {
                                    Status = "Success",
                                    Type = ProblemType.SyzojLegacyTraditional,
                                    ProblemStatement = MessagePackSerializer.Deserialize<LegacySyzojStatement>(psp.Problem._Statement)
                                });
                            }
                            break;
                    }
                    break;
                case "POST":
                    switch(action)
                    {
                        case "edit":
                            if(controllerLevel <= 0)
                            {
                                var serializer = new JsonSerializer();
                                LegacyPutProblemStatementRequest req;
                                using(var sr = new StreamReader(HttpContext.Request.Body))
                                    using(var jsonTextReader = new JsonTextReader(sr))
                                        req = serializer.Deserialize<LegacyPutProblemStatementRequest>(jsonTextReader);
                                LegacySyzojStatement st = MessagePackSerializer.Deserialize<LegacySyzojStatement>(psp.Problem._Statement);
                                st.Description = req.Description;
                                st.InputFormat = req.InputFormat;
                                st.OutputFormat = req.OutputFormat;
                                st.Example = req.Example;
                                st.LimitAndHint = req.LimitAndHint;
                                st.Tags = req.Tags;
                                psp.Problem._Statement = MessagePackSerializer.Serialize<LegacySyzojStatement>(st);
                                await dbContext.SaveChangesAsync();
                                return Ok(new {
                                    Status = "Success",
                                });
                            }
                            break;
                    }
                    break;

            }
            return NotFound(new {
                Status = "Fail",
                Message = "Unsupported action"
            });
        }

        public Task<IActionResult> Submission(ProblemSubmission submission, string action)
        {
            throw new System.NotImplementedException();
        }
    }
}