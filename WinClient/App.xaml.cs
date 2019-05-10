using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DbFramework;
using CellServiceProvider.Models;

namespace WinClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string connString = "Server=127.0.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";
        //const string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

        private DbWpfControls.TableControl.TableControl table;
        //private DbWpfControls.Table table;
        private ProviderContext dbContext;
        private MainWindow mainWindow;

        public App()
        {
            mainWindow = new MainWindow();

            dbContext = new ProviderContext(connString)
            {
                CommandFactory = new NpgsqlCommandFactory(),
                ConnectionFactory = new NpgsqlConnectionFactory(),
            };

            //var users = dbContext.SelectAll<User>();
            var users = dbContext.SelectAll<Service>();

            table = mainWindow.dbGrid;

            table.EntityType = typeof(Service);

            table.DbContext = dbContext;

            table.ItemSelector = () => dbContext.SelectAll<Service>();
            //table.Refresh();

            //table.CellModified += Table_CellModified;

            mainWindow.Show();
        }

        //[Obsolete("Hardcoded primary key.")]
        //private void Table_CellModified(object sender, DbWpfControls.CellModifiedEventArgs e)
        //{
        //    var entity = dbContext.SelectAll<User>().Single(n => n.Id.Equals(e.OldValues["id"]));

        //    entity.InitializeWith(e.NewValues);
        //    entity.Commit();

        //    table.Refresh();
        //}
    }
}
