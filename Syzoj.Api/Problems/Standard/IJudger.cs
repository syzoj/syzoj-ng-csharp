using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Problems.Standard
{
    public interface IJudger
    {
        Task OnSubmission(Guid sessionId, Model.StandardTestData testData);
    }
}