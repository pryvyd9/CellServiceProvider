using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

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
        public HashSet<User> Users { get; } = new HashSet<User>();
        public HashSet<Bill> Bills { get; } = new HashSet<Bill>();

        //internal void Commit(string statement)
        //{
        //    using (var conn = new NpgsqlConnection(connString))
        //    {
        //        conn.Open();

        //        //Debug.WriteLine("CONNECTION SUCCESS");

        //        using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = conn;
        //            cmd.CommandText = statement;
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}


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
    }
}
