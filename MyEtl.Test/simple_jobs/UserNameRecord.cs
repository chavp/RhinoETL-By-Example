using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEtl.Test
{
    using FileHelpers;

    [DelimitedRecord("\t")]
    public class UserNameRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
