using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace DbWpfControls
{
    class Column : StackPanel
    {
        public Table Table { get; }

        public List<Cell> Cells { get; } = new List<Cell>();

        public string ColumnName { get; }


        public Column(string name, Table table)
        {
            ColumnName = name;
            Table = table;
        }

        public void Add(Cell cell)
        {
            Children.Add(cell);
            Cells   .Add(cell);
        }

        public void Clear()
        {
            Children.Clear();
            Cells   .Clear();
            Children.Add(CreateHeader());
        }

        public FrameworkElement CreateHeader()
        {
            var header = new Label() { Content = ColumnName };

            return header;
        }

    }

}
