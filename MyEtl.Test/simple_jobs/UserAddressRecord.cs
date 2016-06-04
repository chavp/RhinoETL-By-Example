using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEtl.Test
{
    using FileHelpers;

    [DelimitedRecord("\t")]
    public class UserAddressRecord
    {
        public int Id { get; set; }
        public string Address { get; set; }
    }
}
