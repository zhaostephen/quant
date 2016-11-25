using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Mixin
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

        public static T SetPropertyValue<T>(this T obj, string property, string value)
        {
            var p = typeof(T).GetProperty(property);
            if(p != null && p.CanWrite)
            {
                p.SetValue(obj, ConvertValue(p.PropertyType, value));
            }
            return obj;
        }

        private static object ConvertValue(Type target, string value)
        {
            if (value == null) return null;

            if(target == typeof(double))
                return value.Double();
            else if (target == typeof(double?))
                return value.DoubleNull();
            else if (target == typeof(int))
                return value.Int();
            else if (target == typeof(int?))
                return value.IntNull();
            else if (target == typeof(DateTime))
                return value.DateTime();
            else if (target == typeof(DateTime?))
                return value.DateTimeNull();

            return value;
        }
    }
}
