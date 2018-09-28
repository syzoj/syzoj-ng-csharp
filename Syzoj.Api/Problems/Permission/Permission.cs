using System.Collections.Generic;

namespace Syzoj.Api.Problems.Permission
{
    public abstract class Permission<T>
        where T : Permission<T>
    {
        private static IDictionary<string, T> Permissions;

        private static void RegisterPermission(T perm)
        {
            Permissions.Add(perm.Name, perm);
        }

        public static T GetPermission(string name)
        {
            T value = null;
            Permissions.TryGetValue(name, out value);
            return value;
        }

        // TODO: Make it read only
        public static IDictionary<string, T> GetAllPermissions()
        {
            return Permissions;
        }

        public string Name { get; }
        public string ErrorMessage { get; }
        public IEnumerable<T> ImpliedPermissions { get; }
        public T ParentPermission { get; }
        public Permission(string name, string errorMessage)
        {
            this.Name = name;
            this.ErrorMessage = errorMessage;
        }

        public Permission(string name, string errorMessage, IEnumerable<T> ImpliedPermissions, T ParentPermission) : this(name, errorMessage)
        {
            this.ImpliedPermissions = ImpliedPermissions;
            this.ParentPermission = ParentPermission;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}