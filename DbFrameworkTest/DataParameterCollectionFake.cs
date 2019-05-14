using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace DbFrameworkTest
{
    public class DataParameterCollectionFake : List<IDbDataParameter>, IDataParameterCollection
    {
        public DataParameterCollectionFake(IEnumerable<IDbDataParameter> col)
        {
            AddRange(col);
        }

        public object this[string parameterName]
        {
            get => this.First(n => n.ParameterName == parameterName);
            set => this[this.IndexOf(this.First(n => n.ParameterName == parameterName))] = (IDbDataParameter)value;
        }

        public bool Contains(string parameterName)
        {
            return this.Any(n => n.ParameterName == parameterName);
        }

        public int IndexOf(string parameterName)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].ParameterName == parameterName)
                {
                    return i;
                }
            }

            return -1;
        }

        public void RemoveAt(string parameterName)
        {
            Remove(this.First(n => n.ParameterName == parameterName));
        }
    }
}
