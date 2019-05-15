using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DbFramework;
using CellServiceProvider.Models;
using System.Windows.Input;
using System.Reflection;

namespace WinClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string connString = "Server=127.0.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";
        //const string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

        //private DbWpfControls.TableControl.TableControl table;
        //private DbWpfControls.DbControl.DbControl dbControl;
        //private DbWpfControls.Table table;
        private ProviderContext dbContext;
        private MainWindow mainWindow;

        public App()
        {
            mainWindow = new MainWindow();
            mainWindow.PreviewKeyDown += MainWindow_PreviewKeyDown;

            dbContext = new ProviderContext(connString)
            {
                CommandFactory = new NpgsqlCommandFactory(),
                ConnectionFactory = new NpgsqlConnectionFactory(),
            };

            //var users = dbContext.SelectAll<User>();
            //var users = dbContext.SelectAll<Service>();

            //table = mainWindow.dbGrid;

            //table.EntityType = typeof(Service);

            //table.DbContext = dbContext;

            //table.ItemSelector = () => dbContext.SelectAll<Service>();


            InitializeDbControl();

            mainWindow.Show();
        }

        private void InitializeDbControl()
        {
            var sources = new Dictionary<string, (Type, DbContext)>
            {
                ["services"] = (typeof(Service), dbContext),
                ["users"] = (typeof(User), dbContext),
                ["user_groups"] = (typeof(UserGroup), dbContext),
                ["users_to_services"] = (typeof(UserToService), dbContext),
                ["bills"] = (typeof(Bill), dbContext),

            };

            mainWindow.dbControl.ScriptAssemblies = new[]
            {
                ("CellServiceProvider.Models", Assembly.GetAssembly(typeof(Service))),
            };

            mainWindow.dbControl.ShowEntities(sources);
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //table.AppKeyDown?.Invoke(sender, e);
            mainWindow.dbControl.AppKeyDown?.Invoke(sender, e);
        }

    }
}
