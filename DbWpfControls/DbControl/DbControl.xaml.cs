using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DbWpfControls.DbControl
{
    /// <summary>
    /// Interaction logic for DbControl.xaml
    /// </summary>
    public partial class DbControl : UserControl
    {
        //private Dictionary<string, Type> entities;

        private readonly List<TableControl.TableControl> tableControls =
            new List<TableControl.TableControl>();

        public DbControl()
        {
            InitializeComponent();

            AppKeyDown += TabControl_PreviewKeyDown;
        }

        public (string, System.Reflection.Assembly)[] ScriptAssemblies { get; set; }


        public KeyEventHandler AppKeyDown;

        public void ShowEntities(IDictionary<string, (Type, DbFramework.DbContext)> entities)
        {
            tableControls.Clear();

            foreach (var (name, (type, dbContext)) in entities.Select(n => (n.Key, n.Value)))
            {

                var tableControl = new TableControl.TableControl()
                {
                    EntityType = type,
                    DbContext = dbContext,
                    ItemSelector = () =>
                        (IEnumerable<DbFramework.Entity>)
                        dbContext
                        .GetType()
                        .GetMethod("SelectAll")
                        .MakeGenericMethod(type)
                        .Invoke(dbContext, null),
                    ScriptAssemblies = ScriptAssemblies,
                };

                tableControls.Add(tableControl);

                tableControl.filterBox.textBox.Text = $"Items().OfType<{type.Name}>()";

                var tab = new TabItem()
                {
                    Header = name,
                    Content = tableControl,
                };

                tabControl.Items.Add(tab);
            }

        }

        private void TabControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            foreach (var tableControl in tableControls)
            {
                tableControl.AppKeyDown?.Invoke(sender, e);
            }
        }


    }
}
