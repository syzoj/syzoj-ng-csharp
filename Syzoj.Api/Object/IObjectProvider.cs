using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;

namespace Syzoj.Api.Object
{
    public interface IObjectProvider
    {
        Task<IObject> GetObject(ApplicationDbContext dbContext, Guid Id);
    }
    public interface IObjectProvider<T> : IObjectProvider
        where T : IObject
    {
    }
}