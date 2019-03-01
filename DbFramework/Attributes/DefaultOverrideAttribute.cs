using System;

namespace DbFramework
{
    public sealed class DefaultOverrideAttribute : Attribute
    {
        public object Value { get; }

        public DefaultOverrideAttribute(object value)
        {
            Value = value;
        }
    }
}
