using System;

namespace CellServiceProvider.Models
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class DefaultAttribute : Attribute
    {
      
    }

    internal sealed class DefaultOverrideAttribute : Attribute
    {
        public object Value { get; }

        internal DefaultOverrideAttribute(object value)
        {
            Value = value;
        }
    }
}
