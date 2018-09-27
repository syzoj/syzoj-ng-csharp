using System.Collections.Generic;

namespace Syzoj.Api.Problems
{
    public abstract class Permission<T>
        where T : Permission<T>
    {

        public string Name { get; }
        public string ErrorMessage { get; }
        public IEnumerable<T> ImpliedPermissions { get; }
        public T ParentPermission { get; }
        public Permission() { }
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