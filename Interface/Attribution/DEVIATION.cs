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
        public DEVIATION(kdata k, int tolerance = 13)
        {
            var macd = new MACD(k);

            var cross_up = cross(macd, (i, next) => i.MACD < 0 && next.MACD >= 0);
            var cross_down = cross(macd, (i, next) => i.MACD >= 0 && next.MACD < 0);
            var close = k.ToDictionary(p => p.date, p => p.close);

            for (var i = 1; i < cross_up.Length; ++i)
            {
                var c1 = cross_up[i].MACD - cross_up[i-1].MACD;
                var c2 = close[cross_up[i].Date] - close[cross_up[i-1].Date];
                var deviated = c1 >= 0 && c2 <= 0;
                if (!deviated)
                    continue;

                var units = k.Count(p => p.date >= cross_up[i-1].Date && p.date <= cross_up[i].Date);
                var deviation = new deviation
                {
                    d1 = cross_up[i-1].Date,
                    d2 = cross_up[i].Date,
                    cross = units
                };

                if (deviation.cross > tolerance)
                    Add(deviation);
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
    }

    public class deviation
    {
        public double cross { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
    }
}
