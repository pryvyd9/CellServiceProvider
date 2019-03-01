using System;

namespace DbFramework
{
   
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; }

        public FieldAttribute(string name)
        {
            Name = name;
        }
    }
}
