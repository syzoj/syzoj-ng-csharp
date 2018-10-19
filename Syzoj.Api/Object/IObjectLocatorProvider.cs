using System;
using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public interface IObjectLocatorProvider
    {
        Task<IObject> GetObject(Uri uri);
        Task<IObjectLocator> GetDefaultObjectLocator();
    }
}