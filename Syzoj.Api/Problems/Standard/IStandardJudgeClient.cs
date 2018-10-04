using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard
{
    public interface IStandardJudgeClient
    {
        Task HandleSubmission(string submissionId, StandardProblemContent content);
    }
}