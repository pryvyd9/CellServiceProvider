using System.Data;

namespace DbFramework
{
    public interface IDbDataAdapter
    {
        IDbCommand SelectCommand { get; }
        IDbCommand CommitCommand { get; }
        IDbCommand DeleteCommand { get; }
    }
}
