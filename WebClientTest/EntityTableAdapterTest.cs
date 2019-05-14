using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebClient;
using CellServiceProvider.Models;
using Moq;
using WebClient.Models;
using System.Linq;
using DbFramework;

namespace WebClientTest
{
    [TestClass]
    public class EntityTableAdapterTest
    {
        [TestMethod]
        public void CreateTable()
        {
            var entities = new Entity[]
            {
                new User(null)
                {
                    NickName = "prizrak",
                    Password = "GAYYYY",
                    GroupId = 1,
                },
                new User(null)
                {
                    Id = 1234567890,
                    NickName = "semantef",
                    Password = "noname12",
                    GroupId = 1,
                },
                new User(null)
                {
                    IsActive = false,
                    NickName = "horse",
                    Password = "lilitan",
                    GroupId = 1,
                },
                new User(null)
                {
                    FullName = "Alex Boongalou",
                    NickName = "tochtoch",
                    Password = "pumpitout",
                    GroupId = 1,
                },
            };

            var adapter = new EntityTableAdapter
            {
                Entities = entities,
            };

            adapter.CreateTable();

            var expected = "<div class=\"db-table\"><div class=\"db-row\"><div class=\"db-cell\"></div><div class=\"db-cell\">prizrak</div><div class=\"db-cell\"></div><div class=\"db-cell\">1</div><div class=\"db-cell\"></div><div class=\"db-cell\">GAYYYY</div></div><div class=\"db-row\"><div class=\"db-cell\">1234567890</div><div class=\"db-cell\">semantef</div><div class=\"db-cell\"></div><div class=\"db-cell\">1</div><div class=\"db-cell\"></div><div class=\"db-cell\">noname12</div></div><div class=\"db-row\"><div class=\"db-cell\"></div><div class=\"db-cell\">horse</div><div class=\"db-cell\"></div><div class=\"db-cell\">1</div><div class=\"db-cell\">False</div><div class=\"db-cell\">lilitan</div></div><div class=\"db-row\"><div class=\"db-cell\"></div><div class=\"db-cell\">tochtoch</div><div class=\"db-cell\">Alex Boongalou</div><div class=\"db-cell\">1</div><div class=\"db-cell\"></div><div class=\"db-cell\">pumpitout</div></div></div>";

            var actual = adapter.Table;

            Assert.AreEqual(expected, actual);
        }
    }
}
