using System;

namespace DbFramework
{
    public struct FieldInfo
    {
        public readonly string Name;
        public readonly bool IsNullable;
        public readonly bool IsKey;
        public readonly bool IsRequired;
        public readonly Type Type;

        public FieldInfo(string name, bool isNullable, bool isRequired, bool isKey, Type type)
        {
            Name = name;
            IsNullable = isNullable;
            IsRequired = isRequired;
            IsKey = isKey;
            Type = type;
        }

        public object InstantiateFieldType(object value)
        {
            var genTypes = Type.GetGenericArguments();

            var constructor = Type.GetConstructor(genTypes);

            var convertedValue = Convert.ChangeType(value, genTypes[0]);

            var instance = constructor.Invoke(new[] { convertedValue });

            return instance;
        }
    }
}
