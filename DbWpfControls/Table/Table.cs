using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DbFramework;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace DbWpfControls
{

    public delegate void CellModifiedEventHandler(object sender, CellModifiedEventArgs e);
    public delegate void CellSelectedEventHandler(object sender);

    public class Table : Grid
    {
        (ColumnDefinition content, ColumnDefinition splitter)[] columnDefinitions;

        readonly List<RowDefinition> rowDefinitions = new List<RowDefinition>();

        readonly List<Entity> showedEntities = new List<Entity>();

        public Func<IEnumerable<Entity>> ItemSelector { get; set; }

        public event CellModifiedEventHandler CellModified;
        public event CellSelectedEventHandler CellSelected;

        public double RowHeight = 24;
        public double ColWidth = 150;
        public double SplitterWidth = 5;

        private bool isSplitterBeingDragged = false;
        private bool isCellBeingModified = false;

        public int Offset = 0;
        public int Count = 1000;

        #region parts

        readonly ScrollViewer headerScrollViewer;
        readonly ScrollViewer contentScrollViewer;

        readonly Grid headerGrid;
        readonly Grid contentGrid;

        Header[] headers { get; set; }

        readonly List<Cell[]> rows = new List<Cell[]>();
        //readonly List<Label> rowIndices = new List<Label>();

        #endregion




        public Table()
        {

            headerGrid = new Grid();

            headerScrollViewer = new ScrollViewer
            {
                Content = headerGrid,
                Background = Brushes.Red,
                Margin = new Thickness(0, 0, SystemParameters.VerticalScrollBarWidth, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            };
            headerScrollViewer.SetValue(RowProperty, 0);
            headerScrollViewer.ScrollChanged += Table_ScrollChanged;



            contentGrid = new Grid();

            contentScrollViewer = new ScrollViewer
            {
                Content = contentGrid,
                Background = Brushes.Blue,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
            };
            contentScrollViewer.SetValue(RowProperty, 1);
            contentScrollViewer.ScrollChanged += Table_ScrollChanged;



            var rowTop = new RowDefinition
            {
                Height = new GridLength(RowHeight)
            };
            var rowBottom = new RowDefinition();

            RowDefinitions.Add(rowTop);
            RowDefinitions.Add(rowBottom);

            Children.Add(headerScrollViewer);
            Children.Add(contentScrollViewer);
        }


        private void SetScrollOffset(double offset)
        {
            headerScrollViewer.ScrollToHorizontalOffset(offset);
            contentScrollViewer.ScrollToHorizontalOffset(offset);
        }

        private void Table_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (isSplitterBeingDragged || isCellBeingModified)
            {
                return;
            }

            SetScrollOffset(e.HorizontalOffset);
        }
      

        private void CreateHeaders(FieldInfo[] infos)
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

        private void CreateColumns(Entity entity)
        {
            var infos = entity.GetFieldInfos();

            columnDefinitions = infos.Select((n,i) =>
            {
                var contentCol = new ColumnDefinition
                {
                    Width = new GridLength(ColWidth)
                };
                var splitterCol = new ColumnDefinition
                {
                    Width = new GridLength()
                };

                // Create Splitters
                GridSplitter createSplitter()
                {
                    var splitter = new GridSplitter
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = SplitterWidth,
                        Background = Brushes.Yellow
                    };

                    // Each second column.
                    splitter.SetCurrentValue(ColumnProperty, i * 2 + 1);
                    splitter.SetCurrentValue(RowSpanProperty, 200000);

                    splitter.DragStarted += (s, e) => isSplitterBeingDragged = true;
                    splitter.DragCompleted += (s, e) => isSplitterBeingDragged = false;

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



        private void DeleteRows()
        {
            rows
                .SelectMany(n => n)
                .ToList()
                .ForEach(n => contentGrid.Children.Remove(n));

            rowDefinitions.Clear();
            rows.Clear();
            contentGrid.RowDefinitions.Clear();
        }

        private void CreateRow(Entity entity)
        {
            var values = entity.GetFieldValues(true);

            var rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(RowHeight);

            rowDefinitions.Add(rowDefinition);
            contentGrid.RowDefinitions.Add(rowDefinition);

            var rowIndex = rows.Count;


            var cells = values.Select((value, cl) =>
            {
                var columnIndex = cl * 2;
                var cell = new Cell(columnIndex, rowIndex, this, value.Value);

                cell.SetValue(Grid.RowProperty, rowIndex);
                cell.SetValue(Grid.ColumnProperty, columnIndex);

                cell.PreviewKeyDown += Cell_KeyDown;
                cell.KeyUp += Cell_KeyUp;
                cell.PreviewMouseDown += Cell_MouseDown;
                return cell;
            }).ToArray();

            rows.Add(cells);
            showedEntities.Add(entity);

            foreach (var cell in cells)
            {
                contentGrid.Children.Add(cell);
            }
        }


        private IReadOnlyDictionary<string, object> GetValues(Cell cell, Func<Cell, object> selector)
        {
            var values = rows[cell.Row]
                .Select(selector)
                .Zip(showedEntities[cell.Row].GetFieldInfos())
                .Select(n => (name: n.Item2.Name, value: n.Item2.InstantiateFieldType(n.Item1)))
                .ToDictionary(n => n.name, n => n.value);

            return values;
        }

        private IReadOnlyDictionary<string,object> GetNewValues(Cell cell)
        {
            return GetValues(cell, n => n.Text);
        }

        internal IReadOnlyDictionary<string, object> GetOldValues(Cell cell)
        {
            return GetValues(cell, n => n.InitializedValue);
        }

        //internal IReadOnlyDictionary<string, object> GetOldValues(int rowId)
        //{
        //    return GetOldValues(rows[rowId][0]);
        //}

        private void Cell_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            isCellBeingModified = true;

            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var cell = (Cell)sender;

                // Paint modified row with yellow.
                foreach (var row in rows[cell.Row])
                {
                    if (row.Background == Brushes.YellowGreen)
                    {
                        continue;
                    }
                    else if (row == cell)
                    {
                        row.Background = Brushes.YellowGreen;
                    }
                    else
                    {
                        row.Background = Brushes.LightGoldenrodYellow;
                    }
                }
              

                var args = new CellModifiedEventArgs(cell.Row, cell.Column, GetOldValues(cell), GetNewValues(cell));

                CellModified?.Invoke(sender, args);

                //if (TryCommit((Cell)sender))
                //{
                //    Refresh();
                //    isCellBeingModified = false;
                //}

                isCellBeingModified = false;

            }
        }

        private void Cell_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            isCellBeingModified = false;
        }

        private void Cell_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CellSelected?.Invoke(sender);
        }


        //internal void SelectRow(Cell cell)
        //{
        //    // Paint modified row with yellow.
        //    foreach (var row in rows[cell.Row])
        //    {
        //        if (row.Background == Brushes.YellowGreen)
        //        {
        //            continue;
        //        }
        //        else if (row == cell)
        //        {
        //            row.Background = Brushes.YellowGreen;
        //        }
        //        else
        //        {
        //            row.Background = Brushes.LightGoldenrodYellow;
        //        }
        //    }
        //}



        public void ShowRows(int offset, int count)
        {
            Offset = offset;
            Count = count;

            showedEntities?.Clear();
            rowDefinitions?.Clear();

            DeleteRows();

            if (ItemSelector is null)
                return;

            var entities = ItemSelector()
                .Skip(offset)
                .Take(count);

            if (!entities.Any())
                return;

            if (headers is null)
            {
                CreateColumns(entities.First());
            }


            foreach (var entity in entities)
            {
                CreateRow(entity);
            }
        }

        public void Refresh()
        {
            ShowRows(Offset, Count);
        }
    }
}
