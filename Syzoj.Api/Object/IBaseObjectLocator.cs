using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public interface IBaseObjectLocator
    {
        string Name { get; }
        Task<IObjectLocator> GetObjectLocator();
    }
}