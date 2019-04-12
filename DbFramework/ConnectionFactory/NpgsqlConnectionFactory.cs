using Npgsql;
//using static DbFramework.Transact;
using System.Data;

namespace DbFramework
{
    public class NpgsqlConnectionFactory : ConnectionFactory
    {
        public override IDbConnection Create(string connString)
        {
            return new NpgsqlConnection(connString);
        }
    }
}
