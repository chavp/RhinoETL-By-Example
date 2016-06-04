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

    public class UserAddressRead : AbstractOperation
    {
        public UserAddressRead(string filePath)
        {
            this.filePath = filePath;
        }

        string filePath = null;

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (var file = FluentFile.For<UserAddressRecord>().From(filePath))
            {
                foreach (object obj in file)
                {
                    yield return Row.FromObject(obj);
                }
            }
        }
    }
}
