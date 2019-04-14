using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbFramework;
using System.Text;

namespace WebClient.Models
{
    public class EntityTableAdapter
    {
        public Entity[] Entities { get; set; }

        public string Header { get; private set; }

        public string Table { get; private set; }

        private string GetCell(string value)
        {
            return $"<div class=\"db-cell\">{value}</div>";
        }

        private string GetRow(FieldInfo[] fields, Entity entity)
        {
            var builder = new StringBuilder();

            var values = entity.GetFieldValues();


            foreach (var field in fields)
            {
                if (values.ContainsKey(field.Name))
                {
                    builder.Append(GetCell(values[field.Name].ToString()));
                }
                else
                {
                    builder.Append(GetCell(string.Empty));
                }
            }

            var row = $"<div class=\"db-row\">{builder.ToString()}</div>";

            return row;
        }

        public void CreateTable()
        {
            if (!Entities.Any())
                return;

            var fields = Entities[0].GetFieldTypes();

            var rows = new List<string>();

            foreach (var entity in Entities)
            {
                rows.Add(GetRow(fields, entity));
            }

            var content = string.Join("", rows);

            var table = $"<div class=\"db-table\">{content}</div>";

            Table = table;
        }
    }
}
