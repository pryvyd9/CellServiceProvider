//using static DbFramework.Transact;
using System.Data;

namespace DbFramework
{
    public abstract class ConnectionFactory
    {
        public abstract IDbConnection Create(string connString);
    }
}
