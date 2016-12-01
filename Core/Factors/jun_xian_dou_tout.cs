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
        public jun_xian_dou_tout(StkDataSeries series)
            : base(series)
        {
            if (series.Any())
            {
                var current = series.Last();

                value = current.MA5 >= current.MA30 
                    && current.MA30 >= current.MA55
                    && current.MA55 >= current.MA120;
            }
        }
    }
}
