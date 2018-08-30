using System.Net.Http;
using System.Threading.Tasks;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models;
using System.IO;

namespace Syzoj.Api.Services
{
    public class ProblemManager : IProblemManager
    {
        public IHttpClientFactory httpClientFactory;
        public ProblemManager(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<Problem> ImportFromLegacySyzoj(string URL)
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
                        problem.DataType = ProblemDataType.SyzojLegacyTraditional;
                        problemData = traditionalProblemData;
                        break;
                    case "submit-answer":
                        var submitAnswerProblemData = new SyzojLegacySubmitAnswerProblemData();
                        problem.DataType = ProblemDataType.SyzojLegacySubmitAnswer;
                        problemData = submitAnswerProblemData;
                        break;
                    case "interaction":
                        var interactionProblemData = new SyzojLegacyInteractionProblemData();
                        interactionProblemData.TimeLimit = syzojProblem.Obj.TimeLimit;
                        interactionProblemData.MemoryLimit = syzojProblem.Obj.MemoryLimit;
                        problem.DataType = ProblemDataType.SyzojLegacyInteraction;
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