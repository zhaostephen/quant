using System;
using System.Collections.Generic;
using System.Text;
using Trade.Data;
using System.Linq;
using Trade.Indicator;

namespace Interace.Attribution
{
    public class DEVIATION : List<deviation>
    {
        public DEVIATION(Series<double> k, deviationtype type, int tolerance = 13)
        {
            var macd = new MACD(k);
            var close = k.ToDictionary(p => p.Date, p => p.Value);

            if (type == deviationtype.底背离)
            {
                var cross_up = cross(macd, (i, next) => i.MACD < 0 && next.MACD >= 0);
                for (var i = 1; i < cross_up.Length; ++i)
                {
                    var c1 = cross_up[i].DIF - cross_up[i - 1].DIF;
                    var c2 = close[cross_up[i].Date] - close[cross_up[i - 1].Date];
                    var deviated = c1 >= 0 && c2 <= 0;
                    if (!deviated)
                        continue;

                    var units = k.Count(p => p.Date >= cross_up[i - 1].Date && p.Date <= cross_up[i].Date);
                    var deviation = new deviation
                    {
                        d1 = cross_up[i - 1].Date,
                        d2 = cross_up[i].Date,
                        cross = units
                    };

                    if (deviation.cross > tolerance)
                        Add(deviation);
                }
            }
            else
            {
                var cross_down = cross(macd, (i, next) => i.MACD >= 0 && next.MACD < 0);
                for (var i = 1; i < cross_down.Length; ++i)
                {
                    var c1 = cross_down[i].DIF - cross_down[i - 1].DIF;
                    var c2 = close[cross_down[i].Date] - close[cross_down[i - 1].Date];
                    var deviated = c1 < 0 && c2 >= 0;
                    if (!deviated)
                        continue;

                    var units = k.Count(p => p.Date >= cross_down[i - 1].Date && p.Date <= cross_down[i].Date);
                    var deviation = new deviation
                    {
                        d1 = cross_down[i - 1].Date,
                        d2 = cross_down[i].Date,
                        cross = units
                    };

                    if (deviation.cross > tolerance)
                        Add(deviation);
                }
            }
        }

        static macd[] cross(MACD k, Func<macd, macd, bool> cmp)
        {
            var cross = new List<macd>();
            for (var i = 1; i < k.Count; ++i)
            {
                if (cmp(k[i-1], k[i]))
                {
                    if (Math.Abs(k[i-1].DEA - k[i-1].DIF) <
                        Math.Abs(k[i].DEA - k[i].DIF))
                        cross.Add(k[i-1]);
                    else
                        cross.Add(k[i]);
                }
            }
            return cross.ToArray();
        }

        public static implicit operator deviation(DEVIATION o)
        {
            return o != null && o.Any() ? o.Last() : null;
        }
    }

    public enum deviationtype
    {
        顶背离,
        底背离
    }

    public class deviation
    {
        public double cross { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
    }
}
