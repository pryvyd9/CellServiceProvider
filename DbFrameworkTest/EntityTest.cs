using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbFramework;
using System.Linq;
using System.Collections.Generic;

namespace DbFrameworkTest
{
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

            var expected = new Dictionary<string, object>
            {
                ["nickname"] = "unjustice",
                ["full_name"] = "Joe",
                ["group_id"] = 11,
                ["password"] = "admin",
            };

            var result = user.GetFieldValues();

            CollectionAssert.AreEquivalent(expected, result.ToDictionary(n => n.Key, n => n.Value));
        }

        [TestMethod]
        public void GetFieldTypes()
        {
            var user = new User(null);

            var expected = new []
            {
                new FieldInfo("nickname",   false, true, false,  typeof(Db<string>)),
                new FieldInfo("id",         false, false, true, typeof(Db<int>)),
                new FieldInfo("is_active",  false, false, false, typeof(Db<bool>)),
                new FieldInfo("full_name",  true,  false, false, typeof(Db<string>)),
                new FieldInfo("group_id",   false, true, false, typeof(Db<int>)),
                new FieldInfo("password",   false, true, false, typeof(Db<string>)),
            };

            var result = user.GetFieldInfos();

            CollectionAssert.AreEquivalent(expected, result);
        }
    }
}
