using System;

namespace DbFramework
{
    public struct FieldInfo
    {
        public readonly string Name;
        public readonly bool IsNullable;
        public readonly Type Type;

        public FieldInfo(string name, bool isNullable, Type type)
        {
            Name = name;
            IsNullable = isNullable;
            Type = type;
        }
    }
}
