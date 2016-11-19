using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Stat
{
    public class statistic
    {
        public string code { get; set; }
        public double? low_to_historical_lowest { get; set; }
        public double? close_up_percent { get; set; }
        public bool jun_xian_dou_tout { get; set; }

        public statistic()
        {

        }
        public statistic(string code)
        {
            this.code = code;
        }
    }

    public class statistic_series : List<statistic>
    {
    }

    public class statistic_value<T>
    {
        public T value { get; set; }

        public statistic_value(StkDataSeries series)
        {
        }

        public override string ToString()
        {
            return value + "";
        }
    }
}
