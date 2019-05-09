using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DbFramework;
using System.Linq;


namespace DbWpfControls
{
    class UpdateInfo
    {
        public Exception Exception { get; internal set; }
    }

    class Cell : TextBox
    {
        readonly int column;
        readonly int row;
        readonly Table table;

        public Cell(int column, int row, Table table)
        {
            this.column = column;
            this.row = row;
            this.table = table;
        }
    }

    class Header : Label
    {
        public Header(string name)
        {
            Content = name;
        }
    }

    public class Table : DockPanel
    {
        (ColumnDefinition content, ColumnDefinition splitter)[] columnDefinitions;

        List<RowDefinition> rowDefinitions;

        List<Entity> showedEntities;

        public Func<IEnumerable<Entity>> ItemSelector { get; set; }


        #region parts

        readonly ScrollViewer scrollViewer;

        readonly StackPanel stackPanel;

        readonly Grid contentGrid;

        readonly Grid headerGrid;

        Header[] headers { get; set; }

        List<Cell[]> rows { get; set; }

        #endregion


        public Table()
        {
            scrollViewer = new ScrollViewer();
            stackPanel = new StackPanel();
            contentGrid = new Grid();

            scrollViewer.Content = stackPanel;

            scrollViewer.SetValue(DockProperty, Dock.Bottom);

            headerGrid = new Grid();

            headerGrid.SetValue(DockProperty, Dock.Top);


            //stackPanel.Children.A
            //stackPanel.Children.A
        }

        void CreateColumns(Entity entity)
        {
            var infos = entity.GetFieldInfos();

            headers = infos.Select(n => new Header(n.Name)).ToArray();

            columnDefinitions = infos.Select(n =>
            {
                var contentCol = new ColumnDefinition();
                var splitterCol = new ColumnDefinition();
                // Create Splitter

                return (contentCol, splitterCol);
            }).ToArray();

            foreach (var (content, splitter) in columnDefinitions)
            {
                headerGrid.ColumnDefinitions.Add(content);
                headerGrid.ColumnDefinitions.Add(splitter);

                contentGrid.ColumnDefinitions.Add(content);
                contentGrid.ColumnDefinitions.Add(splitter);
            }
        }

        public void Refresh()
        {

        }

        public void CreateRow(Entity entity)
        {

            //var infos = entity.GetFieldInfos();
            var values = entity.GetFieldValues();


            var rowDefinition = new RowDefinition();
            rowDefinitions.Add(rowDefinition);

            var rowIndex = rows.Count;


            var cells = values.Select((value, columnIndex) =>
            {
                var cell = new Cell(columnIndex, rowIndex, this)
                {
                    Text = value.Value.ToString(),
                };

                cell.SetValue(Grid.RowProperty, rowDefinition);
                cell.SetValue(Grid.ColumnProperty, columnDefinitions[columnIndex].content);

                return cell;
            }).ToArray();

            rows.Add(cells);
            showedEntities.Add(entity);
        }


        public void ShowRows(int offset, int count)
        {
            showedEntities?.Clear();
            rowDefinitions?.Clear();
            rows?.Clear();

            if (ItemSelector is null)
                return;

            var entities = ItemSelector()
                .Skip(offset)
                .Take(count);

            if (!entities.Any())
                return;

            CreateColumns(entities.First());

            foreach (var entity in entities)
            {
                CreateRow(entity);
            }
        }
    }
}
