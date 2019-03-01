using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Linq;
using System;

namespace CellServiceProvider.Models
{
    public class DbContext
    {
        private readonly string connString;

        //private readonly NpgsqlConnection connection;

        public DbContext(string connString)
        {
            this.connString = connString;

            

        }


        internal void Commit(NpgsqlCommand command)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                command.Connection = conn;
                command.Prepare();
                command.ExecuteNonQuery();
            }
        }

        internal ISet<T> SelectAll<T>() where T : Entity
        {
            var tableAttribute = typeof(T)
                   .GetCustomAttributes(false)
                   .OfType<TableAttribute>()
                   .Single();

            var command = new NpgsqlCommand
            {
                CommandText = $"select * from \"{tableAttribute.Name}\"",
            };

            var entities = Select<T>(command);

            return entities;
        }

        private ISet<T> Select<T>(NpgsqlCommand command) where T : Entity
        {
            HashSet<T> set = new HashSet<T>();

            var type = typeof(T);

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                command.Connection = conn;
                command.Prepare();

                using (var reader = command.ExecuteReader())
                {
                    var values = new Dictionary<string, object>();

                    var properties = type
                        .GetProperties()
                        .Where(n => n.GetCustomAttributes(false).OfType<FieldAttribute>().Only())
                        .ToDictionary(n => n.GetCustomAttributes(false).OfType<FieldAttribute>().Single().Name, n => n);
                    

                    while (reader.Read())
                    {
                        var entity = (T)type
                            .GetConstructors()
                            .Single(n => n
                                .GetParameters()
                                .Only(m => m.ParameterType == GetType()))
                            .Invoke(new[] { this });

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var fieldName = reader.GetName(i);
                            var property = properties[fieldName];

                            var value = reader.GetValue(i);

                            object fieldTypedValue;

                            if (value is DBNull)
                            {
                                fieldTypedValue = property
                                    .PropertyType
                                    .Assembly
                                    .CreateInstance(property.PropertyType.FullName);
                            }
                            else
                            {
                                fieldTypedValue = property
                                    .PropertyType
                                    .GetConstructors()
                                    .Single(n => n
                                        .GetParameters()
                                        .Only(m =>
                                        {
                                            var paramType = property
                                                .PropertyType
                                                .GetGenericArguments()
                                                .Single();

                                            return m.ParameterType
                                                .IsAssignableFrom(property
                                                    .PropertyType
                                                    .GetGenericArguments()
                                                    .Single()
                                                );
                                        }))
                                    .Invoke(new[] { value });
                            }

                            property.SetValue(entity, fieldTypedValue);
                        }

                        set.Add(entity);
                    }

                }
            }

            return set;
        }
    }
}
