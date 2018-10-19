using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public interface IObjectProvider
    {
        Task<IObject> GetObject(Guid Id);
    }
    public interface IObjectProvider<T> : IObjectProvider
        where T : IObject
    {
    }
}