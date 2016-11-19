using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Stat
{
    public class close_up_percent : statistic_value<double?>
    {
        public close_up_percent(StkDataSeries series, TimeSpan cross)
            : this(series, DateTime.Today - cross)
        {

        }
        public close_up_percent(StkDataSeries series, DateTime since)
            : base(series)
        {
            var s = series.Where(p1 => p1.Date >= since);
            if (s.Any())
            {
                var closeup = s.Count(p1 => p1.CloseEnum == CloseEnum.收阳 || p1.CloseEnum == CloseEnum.收平);
                var total = s.Count();

                value = Math.Round((closeup / (double)total) * 100, 2);
            }
        }
    }
}
