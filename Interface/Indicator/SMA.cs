using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class SMA : TimeSeries<double>
    {
        public SMA(TimeSeries<double> data, int N, double initValue = 0d)
        {
            var pre = initValue;
            var a = (double)1 / N;

            for (int i = 0; i < data.Count; i++)
            {
                var value = a * data[i].Value + (1 - a) * pre;

                pre = value;

                Add(new TimePoint<double>(data[i].Date, value));
            }
        }
    }
}
