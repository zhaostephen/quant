using System;
using System.Collections.Generic;
using System.Text;
using Trade.Data;
using System.Linq;

namespace Interace.Attribution
{
    public class DEVIATION : List<deviation>
    {
        public DEVIATION(kdata k, int tolerance = 13)
        {
            var cross_up = cross(k, (i, next) => i.macd < 0 && next.macd >= 0);
            var cross_down = cross(k, (i, next) => i.macd >= 0 && next.macd < 0);

            for (var i = 1; i < cross_up.Count; ++i)
            {
                var c1 = cross_up[i].macd - cross_up[i-1].macd;
                var c2 = cross_up[i].close - cross_up[i-1].close;
                var deviated = c1 >= 0 && c2 <= 0;
                if (!deviated)
                    continue;

                var units = k.Count(p => p.date >= cross_up[i-1].date && p.date <= cross_up[i].date);
                var deviation = new deviation
                {
                    d1 = cross_up[i-1].date,
                    d2 = cross_up[i].date,
                    cross = units
                };

                if (deviation.cross > tolerance)
                    Add(deviation);
            }
        }

        static kdata cross(kdata k, Func<kdatapoint, kdatapoint, bool> cmp)
        {
            var cross = new List<kdatapoint>();
            for (var i = 1; i < k.Count; ++i)
            {
                if (cmp(k[i-1], k[i]))
                {
                    if (Math.Abs(k[i-1].dea.Value - k[i-1].dif.Value) <
                        Math.Abs(k[i].dea.Value - k[i].dif.Value))
                        cross.Add(k[i-1]);
                    else
                        cross.Add(k[i]);
                }
            }
            return new kdata(k.Code, cross);
        }
    }

    public class deviation
    {
        //public double angle { get; set; }
        public double cross { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
    }
}
