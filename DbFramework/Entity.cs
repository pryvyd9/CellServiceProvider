﻿using Npgsql;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Reflection;

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

    public abstract class Entity
    {
        internal DbContext Context { get; }

        protected Entity(DbContext context)
        {
            Context = context;
        }

        //public IDictionary<string, Type> GetFieldTypes()
        //{
        //    var properties = GetType()
        //       .GetProperties()
        //       .Where(n => n
        //           .GetCustomAttributes(false)
        //           .OfType<FieldAttribute>()
        //           .Only()
        //       ).ToDictionary(n => n
        //           .GetCustomAttributes(false)
        //           .OfType<FieldAttribute>()
        //           .Single()
        //           .Name, n => n.PropertyType
        //       );

        //    return properties;
        //}

        public FieldInfo[] GetFieldTypes()
        {
            var properties = GetType()
               .GetProperties()
               .Where(n => n.IsOnly<FieldAttribute>())
               .Select(n =>
               {
                   var name = n
                       .GetCustomAttribute<FieldAttribute>(false)
                       .Name;

                   var isNullable = n
                       .IsDefined(typeof(NullableAttribute));

                   var type = n.PropertyType;

                   var field = new FieldInfo(name, isNullable, type);

                   return field;
               }).ToArray();
               

            return properties;
        }

        public IDictionary<string, object> GetFieldValues()
        {
            var properties = GetType()
                 .GetProperties()
                 .Where(n => n.IsOnly<FieldAttribute>());

            return GetValues(properties);
        }

        internal Dictionary<string, object> GetValues(IEnumerable<PropertyInfo> properties)
        {
            var values = new Dictionary<string, object>();

            foreach (var (property, value) in properties.Select(n => (n, (IDbField)n.GetValue(this))))
            {
                var attributes = property.GetCustomAttributes(false);

                if (!value.IsAssigned)
                {
                    if (Has<DefaultOverrideAttribute>(out var defaultOverrideAttribute))
                    {
                        var defaultValue = defaultOverrideAttribute.Value;

                        if (defaultValue == null)
                        {
                            if (Has<NullableAttribute>(out _))
                            {
                                Add(DBNull.Value);
                            }
                            else
                            {
                                throw new Exception($"Non nullable field {TableName()}.{FieldName()} ({TableClassName()}.{PropertyName()}) cannot have null as default value.");
                            }
                        }
                        else
                        {
                            Add(defaultValue);
                        }
                    }
                    else
                    {
                        if (!Has<DefaultAttribute>(out _))
                        {
                            if (value.IsNull && Has<NullableAttribute>(out _))
                            {
                                Add(DBNull.Value);
                            }
                            else
                            {
                                throw new Exception($"Field {TableName()}.{FieldName()} ({TableClassName()}.{PropertyName()}) with no default value was not assigned.");
                            }
                        }
                    }
                }
                else if (value.IsNull)
                {
                    if (Has<NullableAttribute>(out _))
                    {
                        Add(DBNull.Value);
                    }
                    else
                    {
                        throw new Exception($"Not nullable field {TableName()}.{FieldName()} ({TableClassName()}.{PropertyName()}) was assigned with null.");
                    }
                }
                else
                {
                    Add(value.Value);
                }

                // Functions

                bool Has<T>(out T attribute) => (attribute = attributes.OfType<T>().FirstOrDefault()) != null;

                void Add(object val)
                {
                    var name = attributes.OfType<FieldAttribute>().Single().Name;

                    values.Add(name, val);
                }

                string PropertyName() => property.Name;

                string FieldName() => attributes.OfType<FieldAttribute>().Single().Name;

                string TableClassName() => property.DeclaringType?.FullName;

                string TableName() => property.DeclaringType?.GetCustomAttribute<TableAttribute>(false).Name;
            }

            return values;

        }

        public virtual void Commit()
        {
            Context.Commit(Context.CommandFactory.Commit(this));
        }

        public virtual void Delete()
        {
            Context.Commit(Context.CommandFactory.Delete(this));
        }
    }
}
