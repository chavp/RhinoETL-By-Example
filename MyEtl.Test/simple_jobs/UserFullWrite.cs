using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEtl.Test
{
    using Rhino.Etl.Core;
    using Rhino.Etl.Core.Files;
    using Rhino.Etl.Core.Operations;

    public class UserFullWrite : AbstractOperation
    {
        public UserFullWrite(string filePath)
        {
            this.filePath = filePath;
        }

        string filePath = null;

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            FluentFile engine = FluentFile.For<UserFullRecord>();
            //engine.HeaderText = "Id\tName\tAddress";
            using (FileEngine file = engine.To(filePath))
            {
                foreach (Row row in rows)
                {
                    file.Write(row.ToObject<UserFullRecord>());

                    //pass through rows if needed for another later operation 
                    yield return row;
                }
            }
        }
    }
}
