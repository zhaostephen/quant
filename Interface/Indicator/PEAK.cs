using Interface.Data;
using System;
using System.Collections.Generic;
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
                            type,
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
                            type,
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
                            type,
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
                            type,
                            distance,
                            M);
                        break;
                    }
                default:
                    break;
            }
        }

        private void peak(
            kdata k,
            Func<kdatapoint, kdatapoint, kdatapoint, bool> cmp,
            Func<kdatapoint, sValue<double>> ret,
            crosstype confirmcross,
            PEAK_TYPE type,
            int distance,
            int M)
        {
            var count = k.Count;
            for (var i = distance + 1; i < count - distance; ++i)
            {
                var j = 1;
                for (; j <= distance; ++j)
                {
                    if (!cmp(k[i], k[i - j], k[i + j]))
                        break;
                }
                if (j > distance)
                {
                    Add(ret(k[i]));
                }
            }

            confirm(k, confirmcross, type, M);
        }

        void confirm(kdata k,crosstype confirmcross, PEAK_TYPE type, int M)
        {
            var crosses = new MACD(k.close()).cross();
            var list = new List<sValue<double>>();
            DateTime? lastcrossdate = null;
            foreach(var peak in this)
            {
                var cross = crosses.FirstOrDefault(p => p.value.Date >= peak.Date);
                if (cross != null && cross.type == confirmcross)
                {
                    if (lastcrossdate == cross.value.Date)
                    {
                        switch (type)
                        {
                            case PEAK_TYPE.high:
                                if (peak.Value > list[list.Count - 1].Value)
                                {
                                    list.RemoveAt(list.Count - 1);
                                    list.Add(peak);
                                }
                                break;
                            case PEAK_TYPE.low:
                                if (peak.Value < list[list.Count - 1].Value)
                                {
                                    list.RemoveAt(list.Count - 1);
                                    list.Add(peak);
                                }
                                break;
                        }
                    }
                    else
                    {
                        list.Add(peak);
                    }
                    lastcrossdate = cross.value.Date;
                }
            }

            Clear();
            AddRange(list);
        }

        public static implicit operator double?(PEAK o)
        {
            return o != null && o.Any() ? o.Last().Value : (double?)null;
        }
    }
}
