using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DbFramework;

namespace DbWpfControls.TableControl
{
   

    /// <summary>
    /// Interaction logic for TableControl.xaml
    /// </summary>
    public partial class TableControl : UserControl
    {
        public Func<IEnumerable<Entity>> ItemSelector
        {
            get => table.ItemSelector;
            set
            {
                table.ItemSelector = value;
                table.Refresh();
                insertBox.CreateBoxFor(ItemSelector().FirstOrDefault());
            }
        }

        public Type EntityType { get; set; }
        public DbContext DbContext { get; set; }

        //public int Offset { get => table.Offset; set => table.Offset = value; }
        //public int Count { get => table.Count; set => table.Count = value; }

        /// <summary>
        /// buffer before committing
        /// original values, new values
        /// </summary>
        private readonly Dictionary<FieldDictionary, FieldDictionary> modifiedRows =
            new Dictionary<FieldDictionary, FieldDictionary>();

        //public event CellModifiedEventHandler CellModified { add => table.CellModified += value; remove => table.CellModified -= value; }

        public TableControl()
        {
            InitializeComponent();
            table.CellModified += Table_CellModified;
            insertBox.TableControl = this;
        }

        private void Table_CellModified(object sender, CellModifiedEventArgs e)
        {
            modifiedRows[new FieldDictionary(e.OldValues)] = new FieldDictionary(e.NewValues);
        }

        //public bool TryDelete(int row)
        //{

        //}

        public bool TryCommit(out Exception exception)
        {
            try
            {
                var values = modifiedRows
                    .Select(modifiedRow =>
                    {
                        var entity = ItemSelector()
                            //.FirstOrDefault(m => m.GetKeyValues().All(k => modifiedRow.Key.ContainsKey(k.Key) && modifiedRow.Key[k.Key].Equals(k.Value)));
                            .FirstOrDefault(m => m.GetKeyValues().All(k => modifiedRow.Key[k.Key]?.Equals(k.Value) ?? false));

                        return (entity, oldValues: modifiedRow.Key, newValues: modifiedRow.Value);
                    });

                var foundEntities = values
                    .Where(n => n.entity != null);

                foreach (var (entity, oldValues, newValues) in foundEntities)
                {
                    entity.InitializeWith(newValues);
                    entity.Commit();

                    // Remove entity from buffer.
                    modifiedRows.Remove(oldValues);
                }

                exception = null;

                return true;
            }
            catch (Exception e)
            {
                exception = e;

                return false;
            }
        }

        public bool TryInsert(IReadOnlyDictionary<string, object> values, out Exception exception)
        {
            try
            {
                var constructor = EntityType.GetConstructor(new[] { typeof(DbContext) });

                var instance = (Entity)constructor.Invoke(new[] { DbContext });

                instance.InitializeWith(values);

                instance.Commit();

                exception = null;
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public void Execute(TableCommand command)
        {
            switch (command)
            {
                case RefreshCommand cmd:
                    {
                        Refresh();
                        break;
                    }
                case NextPageCommand cmd:
                    {
                        break;
                    }
                case CommitCommand cmd:
                    {
                        if (!TryCommit(out var exception))
                        {
                            //Console.WriteLine(exception);
                            Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
                        }

                        break;
                    }
                case InsertCommand cmd:
                    {
                        if (!TryInsert(cmd.Values, out var exception))
                        {
                            Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
                        }
                        else
                        {
                            insertBox.Reset();
                        }

                        break;
                    }
            }
        }

        private void Refresh()
        {
            table.Refresh();

            //insertBox.Chil
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Execute(new RefreshCommand());
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            Execute(new CommitCommand());
        }
    }
}
