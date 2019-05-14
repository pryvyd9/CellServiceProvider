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
            int j = 0;

            bool Read()
            {
                if (i >= Data.Length - 1)
                {
                    return false;
                }

                i++;
                j = 0;
                return true;
            }

            reader.Setup(n => n.Read()).Returns(Read);
            reader.Setup(n => n.FieldCount).Returns(() => Data[i].Length);

            reader.Setup(n => n.GetName(0)).Returns(() => (string)Data[i][0][0]);
            reader.Setup(n => n.GetName(1)).Returns(() => (string)Data[i][1][0]);
            reader.Setup(n => n.GetName(2)).Returns(() => (string)Data[i][2][0]);
            reader.Setup(n => n.GetName(3)).Returns(() => (string)Data[i][3][0]);
            reader.Setup(n => n.GetName(4)).Returns(() => (string)Data[i][4][0]);
            reader.Setup(n => n.GetName(5)).Returns(() => (string)Data[i][5][0]);
            reader.Setup(n => n.GetName(6)).Returns(() => (string)Data[i][6][0]);
            reader.Setup(n => n.GetName(7)).Returns(() => (string)Data[i][7][0]);

            reader.Setup(n => n.GetValue(0)).Returns(() => Data[i][0][1]);
            reader.Setup(n => n.GetValue(1)).Returns(() => Data[i][1][1]);
            reader.Setup(n => n.GetValue(2)).Returns(() => Data[i][2][1]);
            reader.Setup(n => n.GetValue(3)).Returns(() => Data[i][3][1]);
            reader.Setup(n => n.GetValue(4)).Returns(() => Data[i][4][1]);
            reader.Setup(n => n.GetValue(5)).Returns(() => Data[i][5][1]);
            reader.Setup(n => n.GetValue(6)).Returns(() => Data[i][6][1]);
            reader.Setup(n => n.GetValue(7)).Returns(() => Data[i][7][1]);


            //reader.Setup(n => n.GetName(j)).Returns(() => (string)Data[i][j][0]);
            //reader.Setup(n => n.GetValue(0)).Returns(() => Data[i][0][1]);
            //reader.Setup(n => n.GetValue(j)).Returns(() => Data[i][j++][1]);
            //reader.Setup(n => n.GetValue(1)).Returns(() => Data[i][1][1]);

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
