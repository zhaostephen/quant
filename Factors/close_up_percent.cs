using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Factors
{
    public class close_up_percent : factor<double?>
    {
        public close_up_percent(kdata series, TimeSpan cross)
            : this(series, DateTime.Today - cross)
        {

        }
        public close_up_percent(kdata series, DateTime since)
            : base(series)
        {
            var s = series.Where(p1 => p1.date >= since);
            if (s.Any())
            {
                var closeup = s.Count(p1 => p1.close>=p1.open);
                var total = s.Count();

                value = Math.Round((closeup / (double)total) * 100, 2);
            }
        }
    }
}
