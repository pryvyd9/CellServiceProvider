using System;

namespace CellServiceProvider.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class DbAttribute : Attribute
    {
        override 
        internal DbAttribute()
        {
            if (meth)
            {

            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class FieldAttribute : Attribute
    {
        internal string Name { get; }

        internal FieldAttribute(string name)
        {
            Name = name;
        }
    }
}
