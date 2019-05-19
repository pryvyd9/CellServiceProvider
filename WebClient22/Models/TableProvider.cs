using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient22.Models
{
    public class TableProvider
    {
        public DbFramework.Entity[] Entities { get; set; }

        public Type EntityType { get; set; }

        public DbFramework.FieldInfo[] FieldInfos { get; set; }

        public Dictionary<string, string> AvailableTables { get; set; }

        //public Dictionary<string, string> InsertValues { get; set; }
    }
}
