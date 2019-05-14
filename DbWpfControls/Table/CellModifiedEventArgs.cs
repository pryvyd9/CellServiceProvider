using System.Collections.Generic;

namespace DbWpfControls
{
    public class CellModifiedEventArgs
    {
        public int Row { get; }
        public int Column { get; }
        public IReadOnlyDictionary<string, object> OldValues { get; }
        public IReadOnlyDictionary<string, object> NewValues { get; }

        public CellModifiedEventArgs(
            int row,
            int column,
            IReadOnlyDictionary<string, object> oldValues, 
            IReadOnlyDictionary<string, object> newValues)
        {
            Row = row;
            Column = column;
            OldValues = oldValues;
            NewValues = newValues;
        }
    }
}
