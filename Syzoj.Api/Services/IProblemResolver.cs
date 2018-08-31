using Syzoj.Api.Controllers;

namespace Syzoj.Api.Services
{
    public interface IProblemResolver
    {
        IProblemController GetController();
        IProblemController GetReadonlyController();
    }
}