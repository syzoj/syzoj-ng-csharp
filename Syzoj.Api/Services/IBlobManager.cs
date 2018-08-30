using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Services
{
    public interface IBlobManager
    {
        Task IncBlobAsync(byte[] Hash);
        Task DecBlobAsync(byte[] Hash);
    }
}