using System.Data;
using System.Collections.Generic;

namespace DbFramework
{
    public abstract class CommandFactory
    {
        public abstract IDbCommand Commit(Entity entity);
        public abstract IDbCommand Delete(Entity entity);
        public abstract IDbCommand Empty();


        protected abstract IDbCommand CreateCommand(string commandString, Dictionary<string, object> values);
    }
}
