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
            var entities = new[]
            {
                new User(null)
                {
                    NickName = "prizrak",
                    Password = "GAYYYY",
                    GroupId = 1,
                },
                new User(null)
                {
                    NickName = "semantef",
                    Password = "noname12",
                    GroupId = 1,
                },
                new User(null)
                {
                    NickName = "horse",
                    Password = "lilitan",
                    GroupId = 1,
                },
                new User(null)
                {
                    NickName = "tochtoch",
                    Password = "pumpitout",
                    GroupId = 1,
                },
            };

            var adapter = new EntityTableAdapter
            {
                Entities = entities.Cast<Entity>().ToList(),
            };

            adapter.CreateTable();

            var result = adapter.Table;


        }
    }
}
