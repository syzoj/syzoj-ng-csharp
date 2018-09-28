namespace Syzoj.Api.Problems.Permission
{
    public class DebugPermissionManager<T> : IPermissionManager<T>
        where T : Permission<T>
    {
        public bool HasPermission(T perm)
        {
            return true;
        }
    }
}