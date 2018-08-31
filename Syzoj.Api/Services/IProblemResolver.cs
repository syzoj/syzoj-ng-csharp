using Syzoj.Api.Controllers;

namespace Syzoj.Api.Services
{
    public interface IProblemResolver
    {
        IProblemController GetProblemFullController();
        IProblemController GetProblemSubmitonlyController();
        IProblemController GetProblemViewonlyController();
        IProblemSubmissionController GetSubmissionController();
    }
}