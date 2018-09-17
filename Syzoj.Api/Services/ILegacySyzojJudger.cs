using System.Threading.Tasks;

namespace Syzoj.Api.Services
{
    public interface ILegacySyzojJudger
    {
        Task SendJudgeRequestAsync(int priority, byte[] data);
    }
}