using System.Threading.Tasks;
using Syzoj.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Syzoj.Api.Services
{
    public class BlobManager : IBlobManager
    {
        private readonly ApplicationDbContext dbContext;
        public BlobManager(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task IncBlobAsync(byte[] Hash)
        {
            var data = await dbContext.BlobMetadata.SingleAsync(bm => bm.Hash == Hash);
            data.ReferenceCount += 1;
            data.TimeAccess = DateTime.Now;
        }
        public async Task DecBlobAsync(byte[] Hash)
        {
            var data = await dbContext.BlobMetadata.SingleAsync(bm => bm.Hash == Hash);
            if(data.ReferenceCount == 0)
                throw new InvalidOperationException("Trying to decrease reference count from zero");
            data.ReferenceCount -= 1;
            data.TimeAccess = DateTime.Now;
        }

        public IQueryable<BlobMetadata> QueryGarbageList(TimeSpan Expiration)
        {
            return dbContext.BlobMetadata
                .Where(bm => bm.ReferenceCount == 0 && bm.TimeAccess <= DateTime.Now - Expiration)
                .OrderBy(bm => bm.TimeAccess);
        }
    }
}