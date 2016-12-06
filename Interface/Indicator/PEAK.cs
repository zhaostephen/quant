using System;
using Trade.Data;

namespace Trade.Indicator
{
    public enum PEAK_TYPE { low, high }
    public class PEAK : TimeSeries<double>
    {
        public PEAK(kdata data, PEAK_TYPE type, int distance = 5)
        {
            switch (type)
            {
                case PEAK_TYPE.low:
                    {
                        peak(data,
                            (a, prev, next) => a.low <= prev.low && a.low <= next.low,
                            p => new TimePoint<double>(p.date, p.low),
                            distance);
                        break;
                    }
                case PEAK_TYPE.high:
                    {
                        peak(data,
                            (a, prev, next) => a.high >= prev.high && a.high >= next.high,
                            p => new TimePoint<double>(p.date, p.high),
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
            Func<kdatapoint, TimePoint<double>> ret,
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
    }
}
