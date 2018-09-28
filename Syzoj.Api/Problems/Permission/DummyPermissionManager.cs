namespace Syzoj.Api.Problems.Permission
{
    public class DummyPermissionManager<T> : IPermissionManager<T>
        where T : Permission<T>
    {
        public bool HasPermission(T perm)
        {
            return false;
        }
    }
}