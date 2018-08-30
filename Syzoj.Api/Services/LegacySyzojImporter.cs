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
using MessagePack;

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
                switch(syzojProblem.Obj.Type)
                {
                    case "traditional":
                        problem.Type = ProblemType.SyzojLegacyTraditional;
                        problem._Data = MessagePackSerializer.Serialize(new SyzojLegacyTraditionalProblemData() {
                            FileIo = syzojProblem.Obj.FileIo,
                            FileIoInputName = syzojProblem.Obj.FileIoInputName,
                            FileIoOutputName = syzojProblem.Obj.FileIoOutputName,
                            TimeLimit = syzojProblem.Obj.TimeLimit,
                            MemoryLimit = syzojProblem.Obj.MemoryLimit,
                        });
                        break;
                    case "submit-answer":
                        problem.Type = ProblemType.SyzojLegacySubmitAnswer;
                        problem._Data = MessagePackSerializer.Serialize(new SyzojLegacySubmitAnswerProblemData());
                        break;
                    case "interaction":
                        problem.Type = ProblemType.SyzojLegacyInteraction;
                        problem._Data = MessagePackSerializer.Serialize(new SyzojLegacyInteractionProblemData() {
                            TimeLimit = syzojProblem.Obj.TimeLimit,
                            MemoryLimit = syzojProblem.Obj.MemoryLimit,
                        });
                        break;
                    default:
                        return null;
                }
                var statement = new ProblemStatement();
                statement.Description = syzojProblem.Obj.Description;
                statement.InputFormat = syzojProblem.Obj.InputFormat;
                statement.OutputFormat = syzojProblem.Obj.OutputFormat;
                statement.Example = syzojProblem.Obj.Example;
                statement.LimitAndHint = syzojProblem.Obj.LimitAndHint;
                statement.Tags = syzojProblem.Obj.Tags;
                problem.Statement = statement;
                return problem;
            }
            else
            {
                return null;
            }
        }
    }
}