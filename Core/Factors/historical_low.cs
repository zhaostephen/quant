using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Factors
{
    public class historical_low : factor<kdatapoint>
    {
        public historical_low(kdata series, DateTime since)
            : base(series)
        {
            var s = series.Where(p1 => p1.date >= since);
            if (s.Any())
            {
                var min = double.MaxValue;
                foreach(var d in s)
                {
                    if (d.low < min)
                    {
                        min = d.low;
                        value = d;
                    }
                }
            }
        }
    }
}
