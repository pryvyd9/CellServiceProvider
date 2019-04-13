using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CellServiceProvider.Models;

namespace CellServiceProvider.Controllers
{
   
    public class DbClientController : Controller
    {
        private void TestEntities()
        {
            string connString = "Server=127.0.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";
            //string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

            var dbContext = new ProviderContext(connString)
            {
                CommandFactory = new DbFramework.NpgsqlCommandFactory(),
                ConnectionFactory = new DbFramework.NpgsqlConnectionFactory(),
            };

            var users = dbContext.SelectAll<User>();
            var users1 = users.Where(n => n.NickName.Value[0] == 'p');

            //users.First().CommitWith(users.ElementAt(1)).Commit();
            //users.First().CommitWith(users.ElementAt(1)).CommitWith("save1").RollBackWith<Exception>().Commit();
        }


        public IActionResult Index()
        {
            //TestEntities();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
