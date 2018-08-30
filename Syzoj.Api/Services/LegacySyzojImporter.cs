using System.Net.Http;
using System.Threading.Tasks;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models;
using System.IO;
using Syzoj.Api.Models.Requests;
using Syzoj.Api.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace Syzoj.Api.Services
{
    public class LegacySyzojImporter : ILegacySyzojImporter
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISessionManager sessionManager;
        private readonly ApplicationDbContext dbContext;
        public LegacySyzojImporter(IHttpClientFactory httpClientFactory, ISessionManager sessionManager, ApplicationDbContext dbContext)
        {
            this.httpClientFactory = httpClientFactory;
            this.sessionManager = sessionManager;
            this.dbContext = dbContext;
        }

        public async Task<Problem> ImportFromLegacySyzojAsync(string URL)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, URL + "/export");
            var client = httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                var syzojProblem = await response.Content.ReadAsAsync<LegacySyzojExport>();
                var problem = new Problem();
                problem.Title = syzojProblem.Obj.Title;
                var problemData = new SyzojLegacyProblemData();
                switch(syzojProblem.Obj.Type)
                {
                    case "traditional":
                        var traditionalProblemData = new SyzojLegacyTraditionalProblemData();
                        traditionalProblemData.FileIo = syzojProblem.Obj.FileIo;
                        traditionalProblemData.FileIoInputName = syzojProblem.Obj.FileIoInputName;
                        traditionalProblemData.FileIoOutputName = syzojProblem.Obj.FileIoOutputName;
                        traditionalProblemData.TimeLimit = syzojProblem.Obj.TimeLimit;
                        traditionalProblemData.MemoryLimit = syzojProblem.Obj.MemoryLimit;
                        problem.Type = ProblemType.SyzojLegacyTraditional;
                        problemData = traditionalProblemData;
                        break;
                    case "submit-answer":
                        var submitAnswerProblemData = new SyzojLegacySubmitAnswerProblemData();
                        problem.Type = ProblemType.SyzojLegacySubmitAnswer;
                        problemData = submitAnswerProblemData;
                        break;
                    case "interaction":
                        var interactionProblemData = new SyzojLegacyInteractionProblemData();
                        interactionProblemData.TimeLimit = syzojProblem.Obj.TimeLimit;
                        interactionProblemData.MemoryLimit = syzojProblem.Obj.MemoryLimit;
                        problem.Type = ProblemType.SyzojLegacyInteraction;
                        problemData = interactionProblemData;
                        break;
                    default:
                        return null;
                }
                problemData.Description = syzojProblem.Obj.Description;
                problemData.InputFormat = syzojProblem.Obj.InputFormat;
                problemData.OutputFormat = syzojProblem.Obj.OutputFormat;
                problemData.Example = syzojProblem.Obj.Example;
                problemData.LimitAndHint = syzojProblem.Obj.LimitAndHint;
                problemData.Tags = syzojProblem.Obj.Tags;
                problem.SetData<object>(problemData);
                return problem;
            }
            else
            {
                return null;
            }
        }
    }
}