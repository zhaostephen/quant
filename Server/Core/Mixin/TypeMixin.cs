using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Mixin
{
    public static class TypeMixin
    {
        public static DateTime TradingDay(this DateTime @this, int daysAdd = 0)
        {
            if (daysAdd == 0)
            {
                while (true)
                {
                    if (@this.DayOfWeek == DayOfWeek.Saturday || @this.DayOfWeek == DayOfWeek.Sunday)
                    {
                        @this = @this.AddDays(-1);
                        continue;
                    }
                    break;
                }
            }
            else
            {
                for (var i = 0; i < Math.Abs(daysAdd); )
                {
                    @this = @this.AddDays(Math.Sign(daysAdd));
                    if (@this.DayOfWeek == DayOfWeek.Saturday || @this.DayOfWeek == DayOfWeek.Sunday)
                        continue;
                    i++;
                }
            }

            return @this;
        }

        public static bool Between(this double @this, double minInclusive, double maxInclusive)
        {
            if(minInclusive > maxInclusive)
            {
                var tmp = minInclusive;
                minInclusive = maxInclusive;
                maxInclusive = tmp;
            }
            return @this >= minInclusive && @this <= maxInclusive;
        }
    }
}
