using System.Collections.Generic;

namespace Syzoj.Api.Problems.Permission
{
    public abstract class PermissionManagerBase<T> : IPermissionManager<T>
        where T : Permission<T>
    {
        protected ISet<T> permissions;

        protected void AddPermission(T perm)
        {
            permissions.Add(perm);
        }

        // TODO: Handle cyclic permission
        public bool HasPermission(T perm)
        {
            return permissions.Contains(perm) || (perm.ParentPermission != null && HasPermission(perm.ParentPermission));
        }
    }
}