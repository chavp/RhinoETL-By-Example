using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyEtl.Test
{
    using Rhino.Etl.Core.ConventionOperations;
    using Rhino.Etl.Core;
    using Rhino.Etl.Core.Operations;
    using System.Collections.Generic;
    using System.IO;
    using System.Data.Common;

    [TestClass]
    public class MyEtlTest
    {
        [TestMethod]
        public void read_client()
        {
            var myMigrateProcess = new MyMigrateProcess();

            myMigrateProcess.Execute();

        }
    }

    public class ReadClients : ConventionInputCommandOperation
    {
        public ReadClients()
            : base("OldSource")
        {
            Command = "SELECT TOP(3000) * FROM client";
        }
    }

    public class ReadMembers : ConventionInputCommandOperation
    {
        public ReadMembers()
            : base("OldSource")
        {
            Command = "SELECT TOP(100) * FROM member";
        }
    }

    public class WriteFileClients : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            string data = string.Empty;
            Directory.CreateDirectory(".\\output");
            foreach (Row row in rows)
            {
                data += string.Format(
                    "{0}|{1}|{2}\n",
                    row["ClientCode"], row["ClientStatus"], row["ClientType"]);

                //pass through rows if needed for2 another later operation 
                yield return row;
            }

            File.WriteAllText(".\\output\\clients.txt", data);
        }
    }

    public class WriteFileMembers : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            string data = string.Empty;
            Directory.CreateDirectory(".\\output");
            foreach (Row row in rows)
            {
                data += string.Format(
                    "{0}|{1}|{2}|{3}" + Environment.NewLine,
                    row["MemID"], row["ContractNo"], row["Name"], row["LastName"]);

                //pass through rows if needed for2 another later operation 
                yield return row;
            }

            File.WriteAllText(".\\output\\members.txt", data);
        }
    }

    public class ReadCommandItems : ConventionInputCommandOperation
    {
        public ReadCommandItems()
            : base("NewSource")
        {
            Command = "SELECT * FROM command_item";
        }
    }
    public class WriteFileCommandItems : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            string data = string.Empty;
            Directory.CreateDirectory(".\\output");
            foreach (Row row in rows)
            {
                data += string.Format(
                    "{0}|{1}|{2}" + Environment.NewLine,
                    row["COMMAND_ITEM_ID"], row["REQUESTED_BY"], row["COMMAND_ITEM_TYPE"]);

                //pass through rows if needed for2 another later operation 
                yield return row;
            }

            File.WriteAllText(".\\output\\commandItems.txt", data);
        }
    }
    public class OutputMembers : AbstractDatabaseOperation
    {
        private readonly string commandText
            = "INSERT INTO member (member_id, name) VALUES(:member_id, :name)";

        DbProviderFactory factory = null;

        public OutputMembers()
            : base("NewSource")
        {
            this.factory = DbProviderFactories.GetFactory("Oracle.DataAccess.Client");

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (var con = this.factory.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                con.ConnectionString = this.ConnectionStringSettings.ConnectionString;
                con.Open();

                cmd.CommandText = this.commandText;

                var idParam = cmd.CreateParameter();
                idParam.ParameterName = ":member_id";

                var nameParam = cmd.CreateParameter();
                nameParam.ParameterName = ":name";

                cmd.Parameters.Add(idParam);
                cmd.Parameters.Add(nameParam);

                using (var tran = con.BeginTransaction())
                {
                    foreach (var row in rows)
                    {
                        idParam.Value = row["MemID"];
                        nameParam.Value = row["Name"];
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            this.Error(ex, "error when add member id: " + idParam.Value);
                            continue;
                        }

                        //Console.WriteLine("Processs - " + idParam.Value);
                        
                        yield return row;
                    }

                    tran.Commit();
                }
            }
        }
    }

    public class MyMigrateProcess : EtlProcess
    {
        protected override void Initialize()
        {
            Register(new ReadMembers());

            Register(new WriteFileMembers());
            Register(new OutputMembers());
        }

        protected override void OnFinishedProcessing(IOperation op)
        {
            base.OnFinishedProcessing(op);

            Console.WriteLine(
                string.Format("OnFinishedProcessing: {0}, Duration: {1}", op.Name, op.Statistics.Duration)
            );

            foreach (var er in op.GetAllErrors())
            {
                Console.WriteLine("\tError: " + er.Message);
            }
        }

        protected override void PostProcessing()
        {
            Console.WriteLine("PostProcessing: Done");
            base.PostProcessing();
        }
    }
}
