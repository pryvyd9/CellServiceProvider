using System.Windows.Controls;
using System.Windows.Input;

namespace DbWpfControls
{
    delegate void CellUpdatedEventHandler(Cell cell);

    class Cell : TextBox
    {
        public Row Row { get; }

        public Column Column { get; }

        public string Value { get => Text; set => Text = value; }

        public event CellUpdatedEventHandler Updated;


        public Cell(Row row, Column column)
        {
            Row    = row;
            Column = column;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Updated?.Invoke(this);
            }
        }
    }

}
