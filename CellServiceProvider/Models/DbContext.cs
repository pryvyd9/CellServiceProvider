using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using System.Text;

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

        public HashSet<UserGroup> UserGroups { get; } = new HashSet<UserGroup>();

        internal void Commit(string statement)
        {

        }
    }

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


            var values = properties
                .ToDictionary(n =>
                {
                    return n
                        .GetCustomAttributes(false)
                        .OfType<FieldAttribute>()
                        .Single()
                        .Name;
                }, n => n.GetValue(this));


            var keyName = properties
                .Single(n => n.GetCustomAttributes(false).OfType<KeyAttribute>().Count() == 1)
                .Name;

            var builder = new StringBuilder()
                .Append($"insert into \"{tableAttribute.Name}\" (")
                .AppendJoin(",", values.Keys.Select(n => $"\"{n}\""))
                .Append(") values (")
                // Inject values
                //.AppendJoin(",", values.Values.Select(n => n is string s ? $"'{s}'" : $"{n}"))
                // Insert question marks
                .AppendJoin(",", values.Values.Select(n => "?"))
                .Append($") on conflict ({keyName}) do update set")
                .AppendJoin(",", values.Keys.Select(n => $"{n} = excluded.{n}"));

            var command = new NpgsqlCommand()
            {
                CommandText = builder.ToString()
            };
            
            context.Commit(builder.ToString());

        }
    }

    [Table("user_groups")]
    public sealed class UserGroup : Entity
    {
        public UserGroup(DbContext context) : base(context)
        {
        }

        [Key("id")]
        public long Id { get; set; }

        [Field("name")]
        public string Name { get; set; }
    }



    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class FieldAttribute : Attribute
    {
        internal string Name { get; }

        internal FieldAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class TableAttribute : Attribute
    {
        internal string Name { get; }

        internal TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class KeyAttribute : FieldAttribute
    {
        internal KeyAttribute(string name) : base(name)
        {

        }
    }
}
