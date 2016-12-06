using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public class kdatapoint
    {
        public DateTime date { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double? macd { get; set; }
        public double? dif { get; set; }
        public double? dea { get; set; }
        public double? ma5 { get; set; }
        public double? ma10 { get; set; }
        public double? ma20 { get; set; }
        public double? ma0 { get; set; }
        public double? ma55 { get; set; }
        public double? ma60 { get; set; }
        public double? ma120 { get; set; }
        public double? ma250 { get; set; }
        public double? peak_l { get; set; }
        public double? peak_h { get; set; }
    }
}
