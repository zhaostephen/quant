using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Mixin
{
    public static class StatisticsMixin
    {
        public static double AVEDEV(this IEnumerable<double> @this)
        {
            var avg = @this.Average();
            var ave = @this.Select(p => Math.Abs(p - avg));
            return ave.Average();
        }
        public static double MA(this IEnumerable<double> @this)
        {
             return @this.Average();
        }
    }
}
