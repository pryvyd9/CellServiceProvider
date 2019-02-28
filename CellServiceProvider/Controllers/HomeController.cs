using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CellServiceProvider.Models;

using Npgsql;

namespace CellServiceProvider.Controllers
{
    public class HomeController : Controller
    {
        private void TestEntities()
        {
            string connString = "Server=172.18.0.1;Port=11;Database=provider;User Id=postgres;Password=admin;";

            DbContext dbContext = new DbContext(connString);

            //var userGroup = new UserGroup(dbContext);

            //userGroup.Id = 12;
            //userGroup.Name = "monk";

            //userGroup.Commit();


            //var user = new User(dbContext)
            //{
            //    Id = 1,
            //    NickName = "prizrak",
            //    GroupId = 12,
            //    //FullName = "Pavlo",
            //    Password = "admin",
            //};

            //user.Commit();

            var bill = new Bill(dbContext)
            {
                UserId = 12,

            };

            bill.Commit();

        }

        private void TestConnection()
        {
            string connString = "Server=172.18.0.1;Port=11;Database=postgres;User Id=postgres;Password=admin;";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                Debug.WriteLine("CONNECTION SUCCESS");

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from my_table;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.WriteLine(reader.GetString(1));
                        }
                    }
                }
            }


        }

        private void Conn_Notification(object sender, NpgsqlNotificationEventArgs e)
        {
            throw new NotImplementedException();
        }

        public IActionResult Index()
        {
            TestConnection();

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
