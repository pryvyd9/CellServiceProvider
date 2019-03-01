using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CellServiceProvider.Models;
using DbFramework;

namespace CellServiceProvider.Controllers
{
    public class HomeController : Controller
    {
        private void TestEntities()
        {
            string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

            DbContext dbContext = new DbContext(connString);

            var users = dbContext.SelectAll<User>().Where(n => n.NickName.Value[0] == 'p');
        }


        public IActionResult Index()
        {
            TestEntities();

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
