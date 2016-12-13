using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Indicator;

namespace Trade.Factors
{
    public class jun_xian_dou_tout : factor<bool>
    {
        public jun_xian_dou_tout(kdata series)
            : base(series)
        {
            if (series.Any())
            {
                var current = series.Last();

                var ma5 = (double)new MA(series.close(),5);
                var ma30 = (double)new MA(series.close(), 30);
                var ma55 = (double)new MA(series.close(), 55);
                var ma120 = (double)new MA(series.close(), 120);

                value = ma5 >= ma30  && ma30 >= ma55 && ma55 >= ma120;
            }
        }
    }
}
