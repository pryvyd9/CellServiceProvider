using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbFramework;
using System.Data;
using System.Linq;
using Moq;

namespace DbFrameworkTest
{
    [TestClass]
    public class DbContextTest
    {
        DbContext context;

        [TestInitialize]
        public void CreateMock()
        {
            context = new DbContext(string.Empty);


            var data = new object[][][]
            {
                new object[][]
                {
                    new object[] { "id", 1 },
                    new object[] { "nickname", "lalamonster" },
                    new object[] { "group_id", 1 },
                    new object[] { "password", "p@ssword" },
                },
                new object[][]
                {
                    new object[] { "id", 2 },
                    new object[] { "nickname", "yuurei" },
                    new object[] { "full_name", "Sam" },
                    new object[] { "group_id", 1 },
                    new object[] { "password", "p@$$w@rd" },
                },
            };

            context.CommandFactory = new NpqlCommandFactoryFake
            {
                Data = data,
            };

            var connectionFactoryMock = new Mock<ConnectionFactory>();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(n => n.Open());

            connectionFactoryMock.Setup(n => n.Create(string.Empty)).Returns(connectionMock.Object);

            context.ConnectionFactory = connectionFactoryMock.Object;
        }

        //void MockData(object[][][] data)
        //{
        //    ((NpqlCommandFactoryFake)context.CommandFactory).Data = data;
        //}

        string GetStatement()
        {
            return ((NpqlCommandFactoryFake)context.CommandFactory).Statement;
        }

        [TestMethod]
        public void SelectAll_TwoValues()
        {
            var result = context.SelectAll<User>();

            Assert.AreEqual(1, (int)result.ElementAt(0).Id);
            Assert.AreEqual("lalamonster", (string)result.ElementAt(0).NickName);

            Assert.AreEqual(2, (int)result.ElementAt(1).Id);
            Assert.AreEqual("yuurei", (string)result.ElementAt(1).NickName);
        }

        [TestMethod]
        public void SelectAll_QueryBuilding_Default()
        {
            _ = context.SelectAll<User>();

            var result = GetStatement();

            Assert.AreEqual("select * from \"users\"", result);
        }


        [TestMethod]
        public void Commit()
        {
            var user = new User(context)
            {
                FullName = "Pavlo",
                GroupId = 1,
                NickName = "prizrak",
                Password = "makarana",
            };
            user.Commit();

            var result = GetStatement();

            var expected = "insert into \"users\" (\"nickname\",\"full_name\",\"group_id\",\"password\") " +
                "values (@nickname,@full_name,@group_id,@password) " +
                "on conflict (\"id\") do " +
                "update set " +
                "\"nickname\" = excluded.\"nickname\"," +
                "\"full_name\" = excluded.\"full_name\"," +
                "\"group_id\" = excluded.\"group_id\"," +
                "\"is_active\" = excluded.\"is_active\"," +
                "\"password\" = excluded.\"password\"";

            Assert.AreEqual(expected, result);
        }

      
    }

    [TestClass]
    public class EntityTest
    {
        [TestMethod]
        public void GetValues()
        {
            var user = new User(null)
            {
                FullName = "Joe",
                NickName = "unjustice",
                GroupId = 11,
                Password = "admin",
            };

            var expected = new System.Collections.Generic.Dictionary<string, object>
            {
                ["nickname"] = "unjustice",
                ["full_name"] = "Joe",
                ["group_id"] = 11,
                ["password"] = "admin",
            };

            var result = user.GetValues();

            CollectionAssert.AreEquivalent(expected, result.ToDictionary(n => n.Key, n => n.Value));
        }

        [TestMethod]
        public void GetFieldTypes()
        {
            var user = new User(null);

            var expected = new System.Collections.Generic.Dictionary<string, System.Type>
            {
                ["nickname"] = typeof(Db<string>),
                ["id"] = typeof(Db<int>),
                ["is_active"] = typeof(Db<bool>),
                ["full_name"] = typeof(Db<string>),
                ["group_id"] = typeof(Db<int>),
                ["password"] = typeof(Db<string>),
            };

            var result = user.GetFieldTypes();

            CollectionAssert.AreEquivalent(expected, result.ToDictionary(n => n.Key, n => n.Value));
        }
    }
}
