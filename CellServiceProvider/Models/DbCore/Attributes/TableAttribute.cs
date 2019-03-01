using System;

namespace CellServiceProvider.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class TableAttribute : Attribute
    {
        internal string Name { get; }

        internal TableAttribute(string name)
        {
            Name = name;
        }
    }
}
