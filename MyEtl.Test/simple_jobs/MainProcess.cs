using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEtl.Test
{
    using Rhino.Etl.Core;

    public class MainProcess : EtlProcess
    {
        string namesFile = string.Empty;
        string addressFile = string.Empty;
        string outputFile = string.Empty;

        public MainProcess(string nameFile, string addressFile, string outputFile)
        {
            this.namesFile = nameFile;
            this.addressFile = addressFile;
            this.outputFile = outputFile;
        }

        protected override void Initialize()
        {
            Register(new JoinUserRecords()
                .Left(new UserNameRead(this.namesFile))
                .Right(new UserAddressRead(this.addressFile))
            );

            Register(new UserFullWrite(this.outputFile));
        }
    }
}
