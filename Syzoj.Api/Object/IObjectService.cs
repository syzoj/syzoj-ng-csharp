using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;

namespace Syzoj.Api.Object
{
    public interface IObjectService
    {
        Task<IObject> GetObject(ApplicationDbContext dbContext, Guid Id);
        Task<Guid> CreateObject<TObjectProvider>(ApplicationDbContext dbContext) where TObjectProvider : IObjectProvider;
        Task DeleteObject(ApplicationDbContext dbContext, Guid Id);
    }
}