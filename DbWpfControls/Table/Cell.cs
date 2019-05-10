using System.Windows.Controls;

namespace DbWpfControls
{
    class Cell : TextBox
    {
        public int Column  { get; }
        public int Row     { get; }
        public Table Table { get; }

        public object InitializedValue { get; }

        public Cell(int column, int row, Table table, object value)
        {
            Column = column;
            Row = row;
            Table = table;
            InitializedValue = value;
            Text = value.ToString();
        }
    }
}
