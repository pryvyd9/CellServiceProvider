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
                table.ItemSelector = () =>
                {
                    try
                    {
                        var result = filterBox.ApplyFilter(value, ScriptAssemblies);
                        //Items().OrderBy(n => ((Service) n).Id)
                        //Items().OfType<Service>().Where(n => !string.IsNullOrWhiteSpace(n.description))

                        return result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return value();
                    }
                };
                

                //table.ItemSelector = value;
                table.Refresh();
                insertBox.CreateBoxFor(EntityType, DbContext);
                //insertBox.CreateBoxFor(ItemSelector().FirstOrDefault());

                BlockableButtons["insert"] = insertBox.InsertButton;
            }
        }

        public Type EntityType { get; set; }
        public DbContext DbContext { get; set; }

        /// <summary>
        /// Assemblies needed for executions scripts
        /// </summary>
        public (string, System.Reflection.Assembly)[] ScriptAssemblies { get; set; }

        public KeyEventHandler AppKeyDown;

        /// <summary>
        /// Buttons to be blocked while command is being executed.
        /// </summary>
        internal Dictionary<string, Button> BlockableButtons { get; }

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

            BlockableButtons = new Dictionary<string, Button>()
            {
                ["delete"] = deleteButton,
                ["refresh"] = refreshButton,
                ["commit"] = commitButton,
            };

        }


        private void Table_CellModified(object sender, CellModifiedEventArgs e)
        {
            modifiedRows[new FieldDictionary(e.OldValues)] = new FieldDictionary(e.NewValues);
        }

        public bool TryDelete(IEnumerable<IReadOnlyDictionary<string, object>> rows, out Exception exception)
        {
            try
            {
                var values = rows
                    .Select(row =>
                    {
                        var entity = ItemSelector()
                            .FirstOrDefault(m => m.GetKeyValues().All(k => row[k.Key]?.Equals(k.Value) ?? false));

                        return (entity, row);
                    });

                var foundEntities = values
                    .Where(n => n.entity != null);

                foreach (var (entity, row) in foundEntities)
                {
                    entity.Delete();
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

        //private void Filter(Func<IEnumerable<Entity>>)

        internal void Refresh()
        {
            //var selector = ItemSelector;

            ////table.ItemSelector = () => filterBox.ApplyFilter(ItemSelector());
            //table.ItemSelector = () =>
            //{
            //    var result = filterBox.ApplyFilter(selector, ScriptAssemblies);
            //    //Items().OrderBy(n => ((Service) n).Id)

            //    return result;
            //};
            //filterBox.ApplyFilter()

            table.Refresh();
        }


        public async void ExecuteWithBlock(TableCommand cmd)
        {
            Enable(false);

            var cancellationToken = new TaskCancellationToken();

            var task = cmd.ExecuteAsync(cancellationToken);

            AppKeyDown += Escape;

            try
            {
                await task;
            }
            catch (Exception e)
            {
                if (!task.IsCanceled)
                {
                    Console.WriteLine(e.Message);
                }
            }

            AppKeyDown -= Escape;

            Enable(true);


            void Enable(bool value)
            {
                foreach (var button in BlockableButtons)
                {
                    button.Value.IsEnabled = value;
                }
            }

            void Escape(object s, KeyEventArgs e1)
            {
                if (e1.Key == Key.Escape)
                {
                    cancellationToken.IsCancellationRequested = true;
                }
            }
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteWithBlock(new RefreshCommand(this));
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteWithBlock(new CommitCommand(this));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteWithBlock(new DeleteCommand(1, this));
        }
    }
}
