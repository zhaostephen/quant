using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Factors
{
    public class historical_low : factor<DataPoint>
    {
        public historical_low(StkDataSeries series, DateTime since)
            : base(series)
        {
            var s = series.Where(p1 => p1.Date >= since);
            if (s.Any())
            {
                var min = double.MaxValue;
                foreach(var d in s)
                {
                    if (d.Low < min)
                    {
                        min = d.Low;
                        value = d;
                    }
                }
            }
        }
    }
}
