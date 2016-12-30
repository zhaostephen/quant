using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class MA : Series<double>
    {
        public MA(Series<double> a, int N)
        {
            for (var i = 0; i < a.Count; i++)
            {
                var sum = 0d;
                var j = 0;
                for (j = 0; j < N && i - j >= 0; ++j)
                    sum += a[i - j].Value;
                Add(a[i].Date, Math.Round(sum / j, 2));
            }
        }

        public static implicit operator double? (MA o)
        {
            return o.Any() ? o.Last().Value : default(double?);
        }
    }
}
