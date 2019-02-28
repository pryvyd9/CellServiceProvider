using Npgsql;
using System.Linq;
using System.Text;
using System;

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


            var values = properties.Select(n => (n, n.GetValue(this)))
            .Where(n =>
            {
                if (n.Item2 is null)
                {
                    var attrs = n.Item1.GetCustomAttributes(false);

                    if (!attrs.OfType<NullableAttribute>().Any())
                    {
                        throw new Exception($"not nullable field {n.Item1.Name} was not assigned.");
                    }

                    var defaultAttribute = attrs.OfType<DefaultAttribute>().FirstOrDefault();

                    if (defaultAttribute != null)
                    {
                        if (defaultAttribute.Value != null)
                        {
                            return true;
                        } 
                    }

                    return false;
                }

                return true;
            })
            .Select(n =>
            {
                var defaultAttribute = n.Item1.GetCustomAttributes(false).OfType<DefaultAttribute>().FirstOrDefault();

                if (defaultAttribute != null)
                {
                    if (defaultAttribute.Value != null)
                    {
                        return (n.Item1, defaultAttribute.Value);
                    }
                }

                return n;
            })
            .ToDictionary(n =>
            {
                return n.Item1
                    .GetCustomAttributes(false)
                    .OfType<FieldAttribute>()
                    .Single()
                    .Name;
            }, n => n.Item2);


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
                .AppendJoin(",", values.Values.Select((n,i) => $"@v{i}"))
                .Append($") on conflict ({string.Join(",", keyNames)}) do update set ")
                .AppendJoin(",", values.Keys.Select(n => $"{n} = excluded.{n}"));



            // Prepare statement

            var command = new NpgsqlCommand()
            {
                CommandText = builder.ToString(),
            };

            var preparedParams = values.Values
                .Select((n, i) => new NpgsqlParameter($"@v{i}", n)).ToArray();

            command.Parameters.AddRange(preparedParams);

            context.Commit(command);
            //context.Commit(builder.ToString());

        }
    }
}
