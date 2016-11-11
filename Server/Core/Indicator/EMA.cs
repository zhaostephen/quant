using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Indicator
{
    public class EMA : TimeSeries<double>
    {
        public EMA(TimeSeries<double> data, int N)
        {
            var prevDMA = 0d;
            var a = 2.0d / (N + 1);
            for (int i = 0; i < data.Count; i++)
            {
                var current = data[i];

                var value = a * current.Value + (1 - a) * prevDMA;
                if (value == 0d)
                    continue;

                prevDMA = value;

                this.Add(new TimePoint<double>
                {
                    Date = current.Date,
                    Value = value,
                });
            }
        }
    }
}
