using System;
using System.Linq;
using DbFramework;
using System.Collections.Generic;

namespace DbWpfControls
{

    class Row
    {
        public Table Table { get; }

        public Cell[] Cells { get; internal set; }

        public Entity Entity { get; }

        public Row(Table table, Entity entity)
        {
            Table = table;
            Entity = entity;

            CreateCells();
        }

        void CreateCells()
        {
            var cells = Entity.GetFieldValues().Values
                .Select((n, i) => {
                    var cell = new Cell(this, Table.Columns[i])
                    {
                        Value = n.ToString()
                    };

                    cell.Updated += Cell_Updated;

                    Table.Columns[i].Add(cell);

                    return cell;
                })
                .ToArray();

            Cells = cells;
        }

        private void Cell_Updated(Cell cell)
        {
            if (!TryCommit(out var info))
            {
                Table.RefreshRows();
            }
        }


        IEnumerable<object> ConvertValues(IEnumerable<(FieldInfo, string)> values)
        {
            foreach (var (info, value) in values)
            {
                var newFieldValue = info.InstantiateFieldType(value);

                yield return newFieldValue;
            }
        }

        bool TryCommit(out UpdateInfo info)
        {
            var values = Cells.Select(n => n.Value);

            var entity = Entity;

            var fieldInfos = entity.GetFieldInfos();

            var convertedValues = ConvertValues(fieldInfos.Zip(values));

            var newFields = fieldInfos
                .Select(n => n.Name)
                .Zip(convertedValues)
                .ToDictionary(n => n.Item1, n => n.Item2);

            entity.InitializeWith(newFields);


            try
            {
                entity.Commit();

                info = new UpdateInfo();

                Table.RefreshRows();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                info = new UpdateInfo()
                {
                    Exception = e,
                };

                return false;
            }

        }
    }

}
