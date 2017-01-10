using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Data
{
    public class fenjibdata
    {
        public string fund_code { get; set; }
        public string fund_name { get; set; }
        public DateTime update_date { get; set; }
        public string stock_code { get; set; }
        public string stock_name { get; set; }
        public double weight { get; set; }
    }
}
