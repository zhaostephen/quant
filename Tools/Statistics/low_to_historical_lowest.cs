using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Stat
{
    public class low_to_historical_lowest : statistic_value<double?>
    {
        public low_to_historical_lowest(StkDataSeries series, DateTime since)
            : base(series)
        {
            var s = series.Where(p1 => p1.Date >= since);
            if (s.Any())
            {
                var current = series.Last();
                var lowest = s.Min(p1 => p1.Low);

                value = Math.Truncate((current.Low / lowest - 1) * 100);
            }
        }
    }
}
