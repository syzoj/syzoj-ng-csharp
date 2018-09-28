namespace Syzoj.Api.Problems
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