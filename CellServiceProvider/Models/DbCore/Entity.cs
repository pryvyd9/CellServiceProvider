using Npgsql;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CellServiceProvider.Models
{
    public abstract class Entity
    {

        private readonly DbContext context;

        protected Entity(DbContext context)
        {
            this.context = context;

            Initialize();
        }

        protected virtual void Initialize()
        {
            // Add entity to dbcontext hashset.

            var property = context
                .GetType()
                .GetProperties()
                .Single(n => n
                    .PropertyType
                    .GetGenericArguments()
                    .Only(m => m == GetType()
                ));

            var addMethod = property
                .PropertyType
                .GetMethod("Add");

            var propVal = property.GetValue(context);

            addMethod.Invoke(propVal, new[] { this });
        }

        //private List<(PropertyInfo, object)> GetValues(IEnumerable<PropertyInfo> properties)
        //{
        //    var values = new List<(PropertyInfo, object)>();

        //    foreach (var (property, value) in properties.Select(n => (n, (IDbField)n.GetValue(this))))
        //    {
        //        var attributes = property.GetCustomAttributes(false);

        //        if (!value.IsAssigned)
        //        {
        //            if (Has<DefaultOverrideAttribute>(out var defaultOverrideAttribute))
        //            {
        //                var defaultValue = defaultOverrideAttribute.Value;

        //                if (defaultValue == null)
        //                {
        //                    if (Has<NullableAttribute>(out var nullableAttribute))
        //                    {
        //                        Add(null);
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("Non nullable field cannot have null as default value.");
        //                    }
        //                }
        //                else
        //                {
        //                    Add(defaultValue);
        //                }
        //            }
        //            else
        //            {
        //                if (Has<DefaultAttribute>(out var defaultAttribute))
        //                {
        //                    continue;
        //                }
        //                else
        //                {
        //                    throw new Exception("Field with no default value was not assigned.");
        //                }
        //            }
        //        }
        //        else if (value.IsNull)
        //        {
        //            if (Has<NullableAttribute>(out var nullableAttribute))
        //            {
        //                Add(null);
        //            }
        //            else
        //            {
        //                throw new Exception("Not nullable field was assigned with null.");
        //            }
        //        }
        //        else
        //        {
        //            Add(value.Value);
        //        }

        //        bool Has<T>(out T attribute) => (attribute = attributes.OfType<T>().FirstOrDefault()) != null;

        //        void Add(object val) => values.Add((property, val));
        //    }

        //    return values;

        //}

        private Dictionary<string, object> GetValues(IEnumerable<PropertyInfo> properties)
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
                                throw new Exception("Non nullable field cannot have null as default value.");
                            }
                        }
                        else
                        {
                            Add(defaultValue);
                        }
                    }
                    else
                    {
                        if (Has<DefaultAttribute>(out _))
                        {
                            continue;
                        }
                        else
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
                    if (Has<NullableAttribute>(out var nullableAttribute))
                    {
                        Add(DBNull.Value);
                    }
                    else
                    {
                        throw new Exception("Not nullable field was assigned with null.");
                    }
                }
                else
                {
                    Add(value.Value);
                }

                bool Has<T>(out T attribute) => (attribute = attributes.OfType<T>().FirstOrDefault()) != null;

                void Add(object val)
                {
                    var name = attributes.OfType<FieldAttribute>().Single().Name;

                    values.Add(name, val);
                }

                string PropertyName() => property.Name;

                string FieldName() => attributes.OfType<FieldAttribute>().Single().Name;

                string TableClassName() => property.DeclaringType.FullName;

                string TableName() => property.DeclaringType.GetCustomAttributes(false).OfType<TableAttribute>().Single().Name;
            }

            return values;

        }


        public virtual void Commit()
        {
            // Create statement

            var tableAttribute = GetType()
                .GetCustomAttributes(false)
                .OfType<TableAttribute>()
                .Single();

            var properties = GetType()
                .GetProperties()
                .Where(n => n
                    .GetCustomAttributes(false)
                    .OfType<FieldAttribute>()
                    .Only()
                );


            var values = GetValues(properties);

            //var values = properties.Select(n => (n, (IDbField)n.GetValue(this)))
            //.Where(n =>
            //{
            //    var value = n.Item2;

            //    var attrs = n.Item1.GetCustomAttributes(false);

            //    if (!value.IsAssigned)
            //    {
            //        var defaultAttribute = attrs.OfType<DefaultAttribute>().FirstOrDefault();

            //        if (defaultAttribute != null)
            //        {
            //            var nullableAttribute = attrs.OfType<NullableAttribute>().FirstOrDefault();

            //            if (defaultAttribute.Value != null)
            //            {
            //                return true;
            //            }
            //            else if (nullableAttribute != null)
            //            {

            //            }
            //        }

            //        return false;
            //    }

            //    if (value.IsNull)
            //    {
            //        if (!attrs.OfType<NullableAttribute>().Any())
            //        {
            //            throw new Exception($"not nullable field {n.Item1.Name} was not assigned.");
            //        }

            //        return true;
            //    }

               

            //    return true;
            //})
            //.Select(n =>
            //{
            //    var value = n.Item2;

            //    var defaultAttribute = n.Item1.GetCustomAttributes(false).OfType<DefaultAttribute>().FirstOrDefault();

            //    if (defaultAttribute != null)
            //    {
            //        if (defaultAttribute.Value != null)
            //        {
            //            return (n.Item1, defaultAttribute.Value);
            //        }
            //    }

            //    return (n.Item1, value.Value);
            //})
            //.ToDictionary(n =>
            //{
            //    return n.Item1
            //        .GetCustomAttributes(false)
            //        .OfType<FieldAttribute>()
            //        .Single()
            //        .Name;
            //}, n => n.Item2);


            //values = values
            //    .Where(n => n.Value != null ? Nullable.GetUnderlyingType(n.Value.GetType()) == null : false)
            //    .ToDictionary(n => n.Key, n => n.Value);

            var keyNames = properties
                .Where(n => n.GetCustomAttributes(false).OfType<KeyAttribute>().Any())
                .Select(n => n.Name);

            //var keyName = properties
            //    .Single(n => n.GetCustomAttributes(false).OfType<KeyAttribute>().Count() == 1)
            //    .Name;

            var builder = new StringBuilder()
                .Append($"insert into \"{tableAttribute.Name}\" (")
                .AppendJoin(",", values.Keys.Select(n => $"\"{n}\""))
                .Append(") values (")
                // Inject values
                //.AppendJoin(",", values.Values.Select(n => n is string s ? $"'{s}'" : $"{n}"))
                // Insert question marks
                .AppendJoin(",", values.Keys.Select(n=>$"@{n}"))
                .Append($") on conflict ({string.Join(",", keyNames)}) do update set ")
                .AppendJoin(",", values.Keys.Select(n => $"{n} = excluded.{n}"));



            // Prepare statement

            var command = new NpgsqlCommand()
            {
                CommandText = builder.ToString(),
            };

            var preparedParams = values
                .Select(n => new NpgsqlParameter($"@{n.Key}", n.Value)).ToArray();

            command.Parameters.AddRange(preparedParams);

            context.Commit(command);
            //context.Commit(builder.ToString());

        }
    }
}
