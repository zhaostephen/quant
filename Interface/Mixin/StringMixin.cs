using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Mixin
{
    public static class StringMixin
    {
        public static DateTime DateTime(this string value)
        {
            DateTime v;
            return System.DateTime.TryParse(value, out v) ? v : System.DateTime.MinValue;
        }
        public static DateTime? DateTimeNull(this string value, DateTime? defaultValue = null)
        {
            DateTime v;
            return System.DateTime.TryParse(value, out v) ? v : defaultValue;
        }
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
        public static double? DoubleNull(this string value, double? defaultValue = null)
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
        public static int Int(this string value, int defaultValue = 0)
        {
            var multilier = 1;
            if (value.EndsWith("亿"))
                multilier = (int)1e9;
            else if (value.EndsWith("万"))
                multilier = (int)1e4;

            value = value.Replace("亿", "").Replace("万", "").Trim();

            int v;
            return int.TryParse(value, out v) ? v * multilier : defaultValue;
        }
        public static int? IntNull(this string value, int? defaultValue = null)
        {
            var multilier = 1;
            if (value.EndsWith("亿"))
                multilier = (int)1e9;
            else if (value.EndsWith("万"))
                multilier = (int)1e4;

            value = value.Replace("亿", "").Replace("万", "").Trim();

            int v;
            return int.TryParse(value, out v) ? v * multilier : defaultValue;
        }
        public static DateTime Date(this string value)
        {
            return System.DateTime.Parse(value);
        }
    }
}
