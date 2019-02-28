using System;

namespace CellServiceProvider.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class KeyAttribute : FieldAttribute
    {
        internal KeyAttribute(string name) : base(name)
        {

        }
    }
}
