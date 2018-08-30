using Syzoj.Api.Models.Runner;

namespace Syzoj.Api.Services
{
    public interface ILegacyRunnerManager
    {
        void SubmitTask(LegacyJudgeRequest req);
    }
}