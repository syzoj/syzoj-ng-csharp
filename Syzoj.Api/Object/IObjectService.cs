using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public interface IObjectService
    {
        Task<IObject> GetObject(Guid Id);
        Task<TObject> CreateObject<TObject, TObjectProvider>(Func<Guid, Task<TObject>> creator)
            where TObject : IObject
            where TObjectProvider : IObjectProvider<TObject>;
    }
}