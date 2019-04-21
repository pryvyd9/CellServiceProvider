using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DbFramework;

namespace DbWpfControls
{

    public class Table : Grid
    {
        internal List<Row> Rows = new List<Row>();

        internal Column[] Columns;

        public IReadOnlyList<int> KeyIndices { get; }

        public int ColumnCount { get; }

        public string Headers { get; }

        public int Offset { get; internal set; }

        public int Limit { get; internal set; } = 100;

        private IEnumerable<Entity> itemSource;

        public IEnumerable<Entity> ItemSource
        {
            get => itemSource;
            set
            {
                itemSource = value;
                CreateColumns();
                RefreshRows();
            }
        }



        void CreateColumns()
        {
            Children.Clear();

            ColumnDefinitions.Clear();

            var firstRow = ItemSource.First();

            var infos = firstRow.GetFieldInfos().ToArray();

            Columns = infos
                .Select(n => new Column(n.Name, this))
                .ToArray();

            int i = 0;

            foreach (var col in Columns)
            {
                var columnDefinition = new ColumnDefinition();

                ColumnDefinitions.Add(columnDefinition);

                col.SetValue(Grid.ColumnProperty, i);

                Children.Add(col);

                col.Children.Add(col.CreateHeader());

                i++;
            }
        }

        void CreateRow(Entity entity)
        {
            var row = new Row(this, entity);

            Rows.Add(row);
        }

        void CreateRows(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                CreateRow(entity);
            }
        }

        public void RefreshRows()
        {
            Rows.Clear();

            foreach (var col in Columns)
            {
                col.Clear();
            }

            CreateRows(ItemSource.Skip(Offset).Take(Limit));
        }
       
    }

}
