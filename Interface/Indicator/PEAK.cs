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
                            (a, prev, next) => a.Low <= prev.Low && a.Low <= next.Low,
                            p => new TimePoint<double>(p.Date, p.Low),
                            distance);
                        break;
                    }
                case PEAK_TYPE.high:
                    {
                        peak(data,
                            (a, prev, next) => a.High >= prev.High && a.High >= next.High,
                            p => new TimePoint<double>(p.Date, p.High),
                            distance);
                        break;
                    }
                default:
                    break;
            }
        }

        static void peak(
            kdata points,
            Func<kdatapoint, kdatapoint, kdatapoint, bool> cmp,
            Action<kdatapoint> add,
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
                    add(points[i]);
            }
        }
    }
}
