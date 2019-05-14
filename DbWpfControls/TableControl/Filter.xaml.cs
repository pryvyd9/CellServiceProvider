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
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Dynamic;

namespace DbWpfControls.TableControl
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter : UserControl
    {
        //public Func<IEnumerable<DbFramework.Entity>, IEnumerable<DbFramework.Entity>> ItemFilter { get; private set; }

        public Filter()
        {
            InitializeComponent();
        }

        //public Func<IEnumerable<dynamic>> Items { get; private set; }
        public Func<IEnumerable<DbFramework.Entity>> Items { get; private set; }



        public IEnumerable<DbFramework.Entity> ApplyFilter(Func<IEnumerable<DbFramework.Entity>> items, (string,Assembly)[] references)
        {
            Items = items;
            string code = String.Join("", references.Select(n => $"using {n.Item1};"));

            code += textBox.Text;

            Assembly coreAssembly = Assembly.GetExecutingAssembly();

            var options = ScriptOptions.Default
                .WithReferences(coreAssembly)
                .WithReferences(references.Select(n => n.Item2))
                .WithImports("System.Linq");
            //var options = ScriptOptions.Default.WithImports("System.Linq");

            var script = CSharpScript.Create(code, options: options, globalsType: typeof(Filter));

            var result = script.RunAsync(globals: this).Result;

            return (IEnumerable<DbFramework.Entity>)result.ReturnValue;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            //CSharpScript.C
        }
    }
}
