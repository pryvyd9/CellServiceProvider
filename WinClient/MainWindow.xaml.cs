using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DbFramework;
using CellServiceProvider.Models;

namespace WinClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DbWpfControls.Table DbTable => dbGrid;

        public MainWindow()
        {
            InitializeComponent();

            //string connString = "Server=127.0.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";
            ////string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

            //var dbContext = new ProviderContext(connString)
            //{
            //    CommandFactory = new DbFramework.NpgsqlCommandFactory(),
            //    ConnectionFactory = new DbFramework.NpgsqlConnectionFactory(),
            //};

            //var users = dbContext.SelectAll<User>();

            //dbGrid.ItemSource = users;
        }
    }
}
