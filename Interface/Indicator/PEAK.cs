using System;
using System.Linq;
using Trade.Data;

namespace Trade.Indicator
{
    public enum PEAK_TYPE { low, high }
    public class PEAK : Series<double>
    {
        public PEAK(kdata data, PEAK_TYPE type, int distance = 5)
        {
            switch (type)
            {
                case PEAK_TYPE.low:
                    {
                        peak(data,
                            (a, prev, next) => a.low <= prev.low && a.low <= next.low,
                            p => new sValue<double>(p.date, p.low),
                            distance);
                        break;
                    }
                case PEAK_TYPE.high:
                    {
                        peak(data,
                            (a, prev, next) => a.high >= prev.high && a.high >= next.high,
                            p => new sValue<double>(p.date, p.high),
                            distance);
                        break;
                    }
                default:
                    break;
            }
        }

        public PEAK(kdata data, Func<kdatapoint,double> f, PEAK_TYPE type, int distance = 5)
        {
            switch (type)
            {
                case PEAK_TYPE.low:
                    {
                        peak(data,
                            (a, prev, next) => f(a) <= f(prev) && f(a) <= f(next),
                            p => new sValue<double>(p.date, f(p)),
                            distance);
                        break;
                    }
                case PEAK_TYPE.high:
                    {
                        peak(data,
                            (a, prev, next) => f(a) >= f(prev) && f(a) >= f(next),
                            p => new sValue<double>(p.date, f(p)),
                            distance);
                        break;
                    }
                default:
                    break;
            }
        }

        private void peak(
            kdata points,
            Func<kdatapoint, kdatapoint, kdatapoint, bool> cmp,
            Func<kdatapoint, sValue<double>> ret,
            int distance = 5)
        {
            var count = points.Count;
            for (var i = distance + 1; i < count - distance; ++i)
            {
                var j = 1;
                for (; j <= distance; ++j)
                {
                    if (!cmp(points[i], points[i - j], points[i + j]))
                        break;
                }
                if (j > distance)
                    Add(ret(points[i]));
            }
        }

        public static implicit operator double?(PEAK o)
        {
            return o != null && o.Any() ? o.Last().Value : (double?)null;
        }
    }
}
