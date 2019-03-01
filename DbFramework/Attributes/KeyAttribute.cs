using System;

namespace DbFramework
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class KeyAttribute : FieldAttribute
    {
        public KeyAttribute(string name) : base(name)
        {

        }
    }
}
