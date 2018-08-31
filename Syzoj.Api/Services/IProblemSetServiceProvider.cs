using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemSetServiceProvider
    {
        IProblemSetService GetProblemSetService(ProblemSet problemSet);
    }
}