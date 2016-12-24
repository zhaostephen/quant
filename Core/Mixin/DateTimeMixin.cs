using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Mixin
{
    public static class DateTimeMixin
    {
        public static DateTime NearestKMinutes(this DateTime @this, DateTime sod, int kminuts, DateTime? eod = null)
        {
            var diff = (@this - sod).TotalMinutes + kminuts - 1.0/60;
            var units = (int)Math.Truncate((diff / kminuts));

            var r = sod.AddMinutes(units * kminuts);

            if (eod.HasValue)
            {
                if (r >= eod)
                    r = eod.Value;
            }

            return r;
        }
    }
}
