using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.backtest
{
    public class pnl
    {
        public string code { get; set; }
        public DateTime date { get; set; }
        public double ratio { get; set; }
        public double value { get; set; }
        public double capital { get; set; }
    }
}
