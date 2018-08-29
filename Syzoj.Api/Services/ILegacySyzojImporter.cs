using System.Threading.Tasks;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Services
{
    public interface ILegacySyzojImporter
    {
        Task<Problem> ImportFromLegacySyzoj(string URL);
    }
}