using System.Threading.Tasks;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models.Requests;

namespace Syzoj.Api.Services
{
    public interface ILegacySyzojImporter
    {
        Task<Problem> ImportFromLegacySyzojAsync(string URL);
    }
}