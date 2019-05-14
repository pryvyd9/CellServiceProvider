using System.Collections.Generic;
using System.Linq;

namespace DbWpfControls.TableControl
{
    internal class FieldDictionary : Dictionary<string, object>
    {
        public FieldDictionary(IReadOnlyDictionary<string, object> obj)
        {
            foreach (var item in obj)
            {
                this[item.Key] = item.Value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is FieldDictionary fd)
            {
                var h = fd.Zip(this).All(n => n.Item1.Value.Equals(n.Item2.Value));
                return h;
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            //return this.Sum(n => n.Key.Sum(m => m) + n.Value.ToString().Sum(m => m));
            return (int)this.Sum(n => n.Key.GetHashCode() + (long)n.Value.GetHashCode());
        }

    }
}
