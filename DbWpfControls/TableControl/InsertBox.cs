using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DbFramework;

namespace DbWpfControls.TableControl
{
    public class InsertBox : StackPanel
    {

        private readonly List<StackPanel> fields = new List<StackPanel>();

        private Entity prototype;

        public TableControl TableControl { get; set; }

        public InsertBox()
        {
            Orientation = Orientation.Horizontal;
        }

        public void Reset()
        {
            CreateBoxFor(prototype);
        }

        public void CreateBoxFor(Entity prototype)
        {
            fields.Clear();
            Children.Clear();

            if (prototype is null)
            {
                return;
            }

            this.prototype = prototype;

            var keys = prototype.GetFieldInfos(Field.Key);
            var nonkeys = prototype.GetFieldInfos(Field.NonKey);


            var insertButton = new Button
            {
                Content = "Insert",
            };

            insertButton.Click += InsertButton_Click;

            Children.Add(insertButton);
            Add(keys, Brushes.Yellow);
            Add(nonkeys, Brushes.White);

            void Add(IEnumerable<FieldInfo> items, Brush brush)
            {
                foreach (var item in items)
                {
                    var field = new StackPanel
                    {
                        Background = brush,
                    };

                    var label = new Label()
                    {
                        Content = item.Name,
                    };

                    var textBox = new TextBox();

                    field.Children.Add(label);
                    field.Children.Add(textBox);

                    fields.Add(field);
                    Children.Add(field);
                }
            }
        }

        private IReadOnlyDictionary<string, object> GetValues()
        {
            var values = fields
               .Select(n => ((TextBox)n.Children[1]).Text)
               .Zip(prototype.GetFieldInfos())
               .Where(n => !string.IsNullOrWhiteSpace(n.Item1))
               .Select(n => (name: n.Item2.Name, value: n.Item2.InstantiateFieldType(n.Item1)))
               .ToDictionary(n => n.name, n => n.value);

            return values;
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            TableControl.Execute(new InsertCommand(GetValues()));
        }
    }
}
