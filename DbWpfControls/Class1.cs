using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DbFramework;

namespace DbWpfControls
{
    class Cell : TextBox
    {
        public Row Row;

        public Column Column;

        public string Value;

    }

    class Column
    {
        public Table Table;

        public List<Cell> Cells;

        public string Name;

        
    }

    class Row
    {
        public Table Table;

        public Cell[] Cells;

    }

    class UpdateInfo
    {

    }

    //delegate bool UpdateCell

    class Table
    {
        List<Row> Rows;
        Column[] Columns;

        public IReadOnlyList<int> KeyIndices { get; }

        public int ColumnCount { get; }

        public string Headers { get; }

        public int Offset { get; private set; }

        public IEnumerable<Entity> ItemSource { get; set; }

        public Table(IEnumerable<Entity> itemSource)
        {
            ItemSource = itemSource;
            var firstRow = itemSource.First();

            firstRow.GetFieldInfos()
        }

        //void CreateRow(

        //void AddRows(string[][] rows)
        //{
        //    var rs = rows.Select(row =>
        //    {
        //        var r = new Row
        //        {
        //            Table = this,
        //        };

        //        var columnIndex = 0;
                
        //        var cells = row.Select(cell =>
        //        {
        //            var c = new Cell()
        //            {
        //                Column = Columns[columnIndex],
        //                Row = r,
        //            };

        //            Columns[columnIndex].Cells.Add(c);
        //            columnIndex++;

        //            return c;
        //        }).ToArray();

        //        r.Cells = cells;

        //        return r;
        //    });

        //    Rows.AddRange(rs);
        //}
        
        bool TryUpdateCell(Cell cell, out UpdateInfo info)
        {
            var row = cell.Row;
            var values = row.Cells.Select(n => n.Value);

            var rowId = Rows.IndexOf(row);



            throw new NotImplementedException();
        }
    }

    public class DataGrid : Grid
    {
        public DataGrid()
        {
             
        }
    }
}
