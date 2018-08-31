using Syzoj.Api.Controllers;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemResolverProvider
    {
        IProblemResolver GetProblemResolver(Problem problem);

    }
}