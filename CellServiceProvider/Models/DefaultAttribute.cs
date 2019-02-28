using System;

namespace CellServiceProvider.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class DefaultAttribute : Attribute
    {
        public object Value { get; }

        internal DefaultAttribute()
        {

        }

        internal DefaultAttribute(object value)
        {
            Value = value;
        }

    }
}
