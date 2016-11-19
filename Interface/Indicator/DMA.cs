using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class DMA : TimeSeries<double>
    {
        public DMA(TimeSeries<double> data, int N)
        {
            var prevDMA = 0d;
            for (int i = 0; i < data.Count; i++)
            {
                var current = data[i];

                var value = N * current.Value + (1 - N) * prevDMA;
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
