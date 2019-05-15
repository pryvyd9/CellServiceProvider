using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DbFramework;
using System;

namespace DbWpfControls.TableControl
{
    public class InsertBox : StackPanel
    {

        private readonly List<StackPanel> fields = new List<StackPanel>();


        private Type type;
        private DbContext dbContext;


        private Entity prototype;

        public TableControl TableControl { get; set; }

        internal Button InsertButton { get; private set; }



        public InsertBox()
        {
            Orientation = Orientation.Horizontal;
        }

        public void Reset()
        {
            CreateBoxFor(type, dbContext);
        }

        public void CreateBoxFor(Type type, DbContext dbContext)
        {
            fields.Clear();
            Children.Clear();

            if (type is null || dbContext is null)
            {
                return;
            }

            this.type = type;
            this.dbContext = dbContext;

            var constructor = type.GetConstructor(new[] { typeof(DbContext) });

            prototype = (Entity)constructor.Invoke(new[] { dbContext });

            var keys = prototype.GetFieldInfos(Field.Key);
            var nonkeys = prototype.GetFieldInfos(Field.NonKey);


            InsertButton = new Button
            {
                Content = "Insert",
            };

            InsertButton.Click += InsertButton_Click;

            Children.Add(InsertButton);
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


        //public void CreateBoxFor(Entity prototype)
        //{
        //    fields.Clear();
        //    Children.Clear();

        //    if (prototype is null)
        //    {
        //        return;
        //    }

        //    this.prototype = prototype;

        //    var keys = prototype.GetFieldInfos(Field.Key);
        //    var nonkeys = prototype.GetFieldInfos(Field.NonKey);


        //    InsertButton = new Button
        //    {
        //        Content = "Insert",
        //    };

        //    InsertButton.Click += InsertButton_Click;

        //    Children.Add(InsertButton);
        //    Add(keys, Brushes.Yellow);
        //    Add(nonkeys, Brushes.White);

        //    void Add(IEnumerable<FieldInfo> items, Brush brush)
        //    {
        //        foreach (var item in items)
        //        {
        //            var field = new StackPanel
        //            {
        //                Background = brush,
        //            };

        //            var label = new Label()
        //            {
        //                Content = item.Name,
        //            };

        //            var textBox = new TextBox();

        //            field.Children.Add(label);
        //            field.Children.Add(textBox);

        //            fields.Add(field);
        //            Children.Add(field);
        //        }
        //    }
        //}

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
            new InsertCommand(TableControl, GetValues()).ExecuteAsync();
        }
    }
}
