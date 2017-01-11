using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Data
{
    public class privatefund
    {
        public string fund_name { get; set; }
        public string stock_code { get; set; }
        public DateTime updatedate { get; set; }
        public string holdtype { get; set; }
        public double amount { get; set; }
        public double percentage { get; set; }
        public string type { get; set; }
        public string changetype { get; set; }
        public double changeamount { get; set; }
    }
}
