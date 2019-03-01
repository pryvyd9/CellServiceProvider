using System;

namespace DbFramework
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DefaultAttribute : Attribute
    {
      
    }
}
