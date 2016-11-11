using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Stat
{
    public class jun_xian_dou_tout : statistic_value<bool>
    {
        public jun_xian_dou_tout(StkDataSeries series)
            : base(series)
        {
            if (series.Any())
            {
                var current = series.Last();

                value = current.MA5 > current.MA30 && current.MA30 > current.MA55;
            }
        }
    }
}
