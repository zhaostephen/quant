using Interface.Data;
using System;
using System.Linq;
using Trade.Data;

namespace Trade.Indicator
{
    public enum PEAK_TYPE { low, high }
    public class PEAK : Series<double>
    {
        public PEAK(kdata data, PEAK_TYPE type, int distance = 5, int M = 3)
        {
            switch (type)
            {
                case PEAK_TYPE.low:
                    {
                        peak(data,
                            (a, prev, next) => a.low <= prev.low && a.low <= next.low,
                            p => new sValue<double>(p.date, p.low),
                            crosstype.gold,
                            distance,
                            M);
                        break;
                    }
                case PEAK_TYPE.high:
                    {
                        peak(data,
                            (a, prev, next) => a.high >= prev.high && a.high >= next.high,
                            p => new sValue<double>(p.date, p.high),
                            crosstype.dead,
                            distance,
                            M);
                        break;
                    }
                default:
                    break;
            }
        }

        public PEAK(kdata data, Func<kdatapoint,double> f, PEAK_TYPE type, int distance = 5, int M = 3)
        {
            switch (type)
            {
                case PEAK_TYPE.low:
                    {
                        peak(data,
                            (a, prev, next) => f(a) <= f(prev) && f(a) <= f(next),
                            p => new sValue<double>(p.date, f(p)),
                            crosstype.gold,
                            distance,
                            M);
                        break;
                    }
                case PEAK_TYPE.high:
                    {
                        peak(data,
                            (a, prev, next) => f(a) >= f(prev) && f(a) >= f(next),
                            p => new sValue<double>(p.date, f(p)),
                            crosstype.dead,
                            distance,
                            M);
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
            crosstype confirmcross,
            int distance,
            int M)
        {
            var crosses = new MACD(points.close()).cross();
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
                {
                    if(confirm(points[i], crosses, confirmcross, M))
                        Add(ret(points[i]));
                }
            }
        }

        bool confirm(kdatapoint k, cross<macd>[] crosses, crosstype confirmcross, int M)
        {
            var cross = crosses.FirstOrDefault(p => p.value.Date >= k.date);
            if (cross != null && cross.type == confirmcross)
                return true;

            return false;
        }

        public static implicit operator double?(PEAK o)
        {
            return o != null && o.Any() ? o.Last().Value : (double?)null;
        }
    }
}
