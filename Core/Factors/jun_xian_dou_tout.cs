using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                value = current.ma5 >= current.ma30 
                    && current.ma30 >= current.ma55
                    && current.ma55 >= current.ma120;
            }
        }
    }
}
