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
        public App()
        {
            var mainWindow = new MainWindow();

            string connString = "Server=127.0.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";
            //string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

            var dbContext = new ProviderContext(connString)
            {
                CommandFactory = new NpgsqlCommandFactory(),
                ConnectionFactory = new NpgsqlConnectionFactory(),
            };

            var users = dbContext.SelectAll<User>();

            mainWindow.dbGrid.ItemSelector = () => dbContext.SelectAll<User>();

            mainWindow.dbGrid.ShowRows(0, 100);

            mainWindow.Show();
        }
    }
}
