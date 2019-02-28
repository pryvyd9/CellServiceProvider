using System;

namespace CellServiceProvider.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableAttribute : Attribute
    {

    }
}
