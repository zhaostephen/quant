using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Mixin
{
    public static class StringMixin
    {
        public static double Double(this string value, double defaultValue = 0.0d)
        {
            var multilier = 1.0d;
            if (value.EndsWith("亿"))
                multilier = 1e9;
            else if (value.EndsWith("万"))
                multilier = 1e4;

            value = value.Replace("亿", "").Replace("万", "").Trim();

            double v;
            return double.TryParse(value, out v) ? v * multilier : defaultValue;
        }
        public static DateTime Date(this string value)
        {
            return DateTime.Parse(value);
        }
    }
}
