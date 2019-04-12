using DbFramework;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Moq;

namespace DbFrameworkTest
{
    public class NpqlCommandFactoryFake : NpgsqlCommandFactory
    {
        public object[][][] Data;

        public string Statement;

        private Mock<IDataReader> GetReader()
        {
            var reader = new Mock<IDataReader>();

            int i = -1;

            bool Read()
            {
                if (i >= Data.Length - 1)
                {
                    return false;
                }

                i++;
                return true;
            }

            reader.Setup(n => n.Read()).Returns(Read);
            reader.Setup(n => n.FieldCount).Returns(2);
            reader.Setup(n => n.GetName(0)).Returns(() => (string)Data[i][0][0]);
            reader.Setup(n => n.GetName(1)).Returns(() => (string)Data[i][1][0]);
            reader.Setup(n => n.GetValue(0)).Returns(() => Data[i][0][1]);
            reader.Setup(n => n.GetValue(1)).Returns(() => Data[i][1][1]);

            return reader;
        }

        protected override IDbCommand CreateCommand(string commandString, Dictionary<string, object> values)
        {
            var command = new Mock<IDbCommand>();

            command.SetupSet(n => n.CommandText).Callback(n => Statement = n);

            command.Object.CommandText = commandString;

            var reader = GetReader();
            command.Setup(n => n.ExecuteReader()).Returns(reader.Object);

            var preparedParams = values
                .Select(n => new NpgsqlParameter($"@{n.Key}", n.Value))
                .ToArray();

            command.Setup(n => n.Parameters)
                .Returns(new DataParameterCollectionFake(preparedParams));

            return command.Object;
        }

        public override IDbCommand Empty()
        {
            var command = new Mock<IDbCommand>();

            var reader = GetReader();

            command.Setup(n => n.ExecuteReader()).Returns(reader.Object);

            command.SetupSet(n => n.CommandText).Callback(n => Statement = n);
         

            return command.Object;
        }
    }
}
