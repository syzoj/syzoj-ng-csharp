using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public interface IObjectService
    {
        Task<IObject> GetObject(Guid Id);
        Task<Guid> CreateObject<TObjectProvider>() where TObjectProvider : IObjectProvider;
        Task DeleteObject(Guid Id);
    }
}