using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbFramework;

namespace CellServiceProvider.Models
{
    public class ProviderContext : DbContext
    {
        public ProviderContext(string connString) : base(connString)
        {
        }
    }
}
