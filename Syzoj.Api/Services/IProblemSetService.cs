using System.Threading.Tasks;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemSetService
    {
        Task HandleNewSubmission(ProblemSubmission submission);
        Task HandleFinishedSubmission(ProblemSubmission submission);
    }
}