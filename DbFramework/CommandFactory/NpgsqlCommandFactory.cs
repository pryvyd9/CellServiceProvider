using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System;
using System.Data;
using System.Text;

namespace DbFramework
{
    public class NpgsqlCommandFactory : CommandFactory
    {
        public override IDbCommand Commit(Entity entity)
        {
            // Create statement

            var tableAttribute = entity.GetType()
                .GetCustomAttributes(false)
                .OfType<TableAttribute>()
                .Single();

            var properties = entity.GetType()
                .GetProperties()
                .Where(n => n
                    .GetCustomAttributes(false)
                    .OfType<FieldAttribute>()
                    .Only()
                )
                .ToArray();

            var values = entity.GetValues(properties);

            var keys = properties
                .Where(n => n.GetCustomAttributes(false).OfType<KeyAttribute>().Any())
                .ToArray();

            var keyNames = keys
                .Select(n => n.GetCustomAttributes(false).OfType<KeyAttribute>().Single().Name);

            var nonKeyNames = properties
                .Except(keys)
                .Select(n => n.GetCustomAttributes(false).OfType<FieldAttribute>().Single().Name)
                .ToArray();

            var builder = new StringBuilder()
                .Append($"insert into \"{tableAttribute.Name}\" (")
                .AppendJoin(",", values.Keys.Select(n => $"\"{n}\""))
                .Append(") values (")
                .AppendJoin(",", values.Keys.Select(n => $"@{n}"))
                .Append(")");

            if (nonKeyNames.Any())
            {
                builder
                    .Append($" on conflict (")
                    .AppendJoin(",", keyNames.Select(n => $"\"{n}\""))
                    .Append(") do update set ")
                    .AppendJoin(",", nonKeyNames.Select(n => $"\"{n}\" = excluded.\"{n}\""));
            }

            return CreateCommand(builder.ToString(), values);
        }

        public override IDbCommand Delete(Entity entity)
        {
            // Create statement

            var tableAttribute = entity.GetType()
                .GetCustomAttributes(false)
                .OfType<TableAttribute>()
                .Single();


            var keys = entity.GetType()
                .GetProperties()
                .Where(n => n
                    .GetCustomAttributes(false)
                    .OfType<KeyAttribute>()
                    .Only()
                );

            var values = entity.GetValues(keys);

            var builder = new StringBuilder()
                .Append($"delete from \"{tableAttribute.Name}\" where ")
                .AppendJoin(" and ", values.Select(n => $"\"{n.Key}\" = @{n.Key}"));


            // Prepare statement

            return CreateCommand(builder.ToString(), values);
        }

        public override IDbCommand Empty()
        {
            return new NpgsqlCommand();
        }



        protected override IDbCommand CreateCommand(string commandString, Dictionary<string, object> values)
        {
            var command = new NpgsqlCommand
            {
                CommandText = commandString,
            };

            var preparedParams = values
                .Select(n => new NpgsqlParameter($"@{n.Key}", n.Value)).ToArray();

            command.Parameters.AddRange(preparedParams);

            return command;
        }
    }
}
