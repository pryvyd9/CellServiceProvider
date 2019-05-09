using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DbFramework;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

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

    public class Table : Grid
    {
        (ColumnDefinition content, ColumnDefinition splitter)[] columnDefinitions;
        (ColumnDefinition content, ColumnDefinition splitter)[] contentColumnDefinitions;

        readonly List<RowDefinition> rowDefinitions = new List<RowDefinition>();

        readonly List<Entity> showedEntities = new List<Entity>();

        public Func<IEnumerable<Entity>> ItemSelector { get; set; }

        public double RowHeight = 24;
        public double SplitterWidth = 5;

        #region parts

        readonly ScrollViewer scrollViewer;

        readonly Grid contentGrid;

        readonly Grid headerGrid;

        Header[] headers { get; set; }

        readonly List<Cell[]> rows = new List<Cell[]>();

        #endregion


        public Table()
        {
            contentGrid = new Grid();

            headerGrid = new Grid
            {
                Background = Brushes.Red,
                Margin = new Thickness(0, 0, SystemParameters.VerticalScrollBarWidth, 0)
            };
            headerGrid.SetValue(RowProperty, 0);

            scrollViewer = new ScrollViewer
            {
                Content = contentGrid,
                Background = Brushes.Blue
            };
            scrollViewer.SetValue(RowProperty, 1);


            var rowTop = new RowDefinition
            {
                Height = new GridLength(RowHeight)
            };
            var rowBottom = new RowDefinition();

            RowDefinitions.Add(rowTop);
            RowDefinitions.Add(rowBottom);

            Children.Add(scrollViewer);
            Children.Add(headerGrid);

        }

        void CreateHeaders(FieldInfo[] infos)
        {
            headers = infos.Select(n => new Header(n.Name)).ToArray();

            headers.Select((n, i) =>
            {
                n.SetCurrentValue(ColumnProperty, i * 2);
                return n;
            }).ToList()
            .ForEach(n =>
            {
                headerGrid.Children.Add(n);
            });

        }

        void CreateColumns(Entity entity)
        {
            var infos = entity.GetFieldInfos();

            columnDefinitions = infos.Select((n,i) =>
            {
                var contentCol = new ColumnDefinition();
                var splitterCol = new ColumnDefinition
                {
                    Width = new GridLength()
                };
                // Create Splitter

                GridSplitter createSplitter()
                {
                    var splitter = new GridSplitter
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Width = SplitterWidth,
                        Background = Brushes.Yellow
                    };
                    splitter.SetCurrentValue(ColumnProperty, i * 2 + 1);
                    splitter.SetCurrentValue(RowSpanProperty, 200000);
                    return splitter;
                }

                contentGrid.Children.Add(createSplitter());
                headerGrid.Children.Add(createSplitter());
                

                return (contentCol, splitterCol);
            }).ToArray();

            foreach (var (content, splitter) in columnDefinitions)
            {
                headerGrid.ColumnDefinitions.Add(content);
                headerGrid.ColumnDefinitions.Add(splitter);

                var contentContentCol = new ColumnDefinition();
                {
                    var binding = new Binding
                    {
                        Source = content,
                        Path = new PropertyPath("Width"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    BindingOperations.SetBinding(contentContentCol, ColumnDefinition.WidthProperty, binding);
                }
                var contentSplitterCol = new ColumnDefinition();
                {
                    var binding = new Binding
                    {
                        Source = splitter,
                        Path = new PropertyPath("Width"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    BindingOperations.SetBinding(contentSplitterCol, ColumnDefinition.WidthProperty, binding);
                }

                contentGrid.ColumnDefinitions.Add(contentContentCol);
                contentGrid.ColumnDefinitions.Add(contentSplitterCol);

            }

            CreateHeaders(infos);
        }

        public void Refresh()
        {

        }

        public void CreateRow(Entity entity)
        {

            //var infos = entity.GetFieldInfos();
            var values = entity.GetFieldValues();


            var rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(RowHeight);

            rowDefinitions.Add(rowDefinition);
            contentGrid.RowDefinitions.Add(rowDefinition);

            var rowIndex = rows.Count;


            var cells = values.Select((value, cl) =>
            {
                var columnIndex = cl * 2;
                var cell = new Cell(columnIndex, rowIndex, this)
                {
                    Text = value.Value.ToString(),
                };

                cell.SetValue(Grid.RowProperty, rowIndex);
                cell.SetValue(Grid.ColumnProperty, columnIndex);

                return cell;
            }).ToArray();

            rows.Add(cells);
            showedEntities.Add(entity);

            foreach (var cell in cells)
            {
                contentGrid.Children.Add(cell);
            }
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
