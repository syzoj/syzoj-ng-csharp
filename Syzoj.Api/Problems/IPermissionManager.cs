using System.Threading.Tasks;

namespace Syzoj.Api.Problems
{
    /// <summary>
    /// This interface defines whether a given identity should be able to perform certain actions.
    /// </summary>
    public interface IPermissionManager<T>
        where T : Permission<T>
    {
        bool HasPermission(T perm);
    }
}