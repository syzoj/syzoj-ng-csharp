using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public interface IObjectLocator : IObject
    {
        /// <remarks>
        /// The segment is guarenteed to end with '/'.
        /// </remarks>
        Task<IObjectLocator> GetObjectLocator(string segment);
        Task<IObject> GetObject(string segment);
    }
}