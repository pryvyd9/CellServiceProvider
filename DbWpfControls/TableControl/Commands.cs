using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbWpfControls.TableControl
{
    public abstract class TableCommand { }

    public class RefreshCommand : TableCommand { }

    public class NextPageCommand : TableCommand { }

    public class CommitCommand : TableCommand { }

    public class InsertCommand : TableCommand
    {
        public IReadOnlyDictionary<string, object> Values { get; }

        public InsertCommand(IReadOnlyDictionary<string, object> values)
        {
            Values = values;
        }
    }
}
