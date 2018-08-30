using System.Threading.Tasks;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface IProblemManager
    {
        Task<Problem> ImportFromLegacySyzojAsync(string URL);
    }
}