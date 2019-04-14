using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Linq;
using System;
//using static DbFramework.Transact;
using System.Data;
using System.Reflection;

namespace DbFramework
{
    public class DbContext
    {
        private readonly string _connString;

        public CommandFactory CommandFactory;

        public ConnectionFactory ConnectionFactory;


        public DbContext(string connString)
        {
            this._connString = connString;
        }

        internal void Commit(IDbCommand command)
        {
            using (var conn = ConnectionFactory.Create(_connString))
            {
                conn.Open();

                command.Connection = conn;
                command.Prepare();
                command.ExecuteNonQuery();
            }
        }



        public ISet<T> SelectAll<T>() where T : Entity
        {
            var tableAttribute = typeof(T)
                   .GetCustomAttributes(false)
                   .OfType<TableAttribute>()
                   .Single();

            var command = CommandFactory.Empty();

            command.CommandText = $"select * from \"{tableAttribute.Name}\"";

            var entities = Select<T>(command);

            return entities;
        }

        public void DeleteAll<T>()
        {
            var tableAttribute = typeof(T)
                  .GetCustomAttributes(false)
                  .OfType<TableAttribute>()
                  .Single();

            var command = new NpgsqlCommand
            {
                CommandText = $"delete from \"{tableAttribute.Name}\"",
            };

            Delete(command);
        }




        private ISet<T> Select<T>(IDbCommand command) where T : Entity
        {
            HashSet<T> set = new HashSet<T>();

            var type = typeof(T);

            using (var conn = ConnectionFactory.Create(_connString))
            {
                conn.Open();

                command.Connection = conn;
                command.Prepare();

                using (var reader = command.ExecuteReader())
                {
                    var values = new Dictionary<string, object>();

                    var properties = type
                        .GetProperties()
                        .Where(n => n.GetCustomAttributes<FieldAttribute>(false).Only())
                        .ToDictionary(n => n.GetCustomAttribute<FieldAttribute>(false).Name, n => n);
                    

                    while (reader.Read())
                    {
                        var entity = (T)type
                            .GetConstructors()
                            .Single(n => n
                                .GetParameters()
                                .Only(m => m.ParameterType.IsAssignableFrom(GetType())))
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

        private void Delete(IDbCommand command)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();

                command.Connection = conn;
                command.Prepare();

                command.ExecuteNonQuery();
            }
        }
    }
}
