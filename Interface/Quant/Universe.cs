using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Quant
{
    public class universe
    {
        public string name { get; set; }
        public string[] codes { get; set; }

        public universe()
        {

        }
        public universe(string name, string[] codes)
        {
            this.name = name;
            this.codes = codes;
        }
    }
}
