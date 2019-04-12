using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbFramework;

using Moq;

namespace DbFrameworkTest
{
    [TestClass]
    public class DbTest
    {

        [TestMethod]
        public void AssignDefaultValue_IsAssigned_False()
        {
            Db<object> field = default;

            Assert.IsFalse(field.IsAssigned);
        }

        [TestMethod]
        public void AssignValue_IsAssigned_True()
        {
            Db<int> field = 12;

            Assert.IsTrue(field.IsAssigned);
        }

        [TestMethod]
        public void InitializeVariableWithDbValue_AreEqual_True()
        {
            Db<int> field = 12;

            int value = field;

            Assert.AreEqual(value, field.Value);
        }
    }
}
