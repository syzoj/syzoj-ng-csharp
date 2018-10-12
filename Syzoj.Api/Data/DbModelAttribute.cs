using System;

namespace Syzoj.Api.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DbModelAttribute : Attribute
    {
    }
}