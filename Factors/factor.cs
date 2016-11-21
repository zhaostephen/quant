using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Factors
{
    public class factorset
    {
        public string code { get; set; }
        public double? low_to_historical_lowest { get; set; }
        public double? close_up_percent { get; set; }
        public bool jun_xian_dou_tout { get; set; }

        public factorset()
        {

        }
        public factorset(string code)
        {
            this.code = code;
        }
    }

    public class factor<T>
    {
        public T value { get; set; }

        public factor(StkDataSeries series)
        {
        }

        public override string ToString()
        {
            return value + "";
        }
    }
}
