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

            for (var i = 0; i < cross_up.Count - 1; ++i)
            {
                var c1 = cross_up[i + 1].macd - cross_up[i].macd;
                var c2 = cross_up[i + 1].close - cross_up[i].close;
                var deviated = c1 >= 0 && c2 <= 0;
                if (!deviated)
                    continue;

                //var k1 = (cross_up[i + 1].close - cross_up[i].close) / dates;
                //var k2 = (cross_up[i + 1].dif.Value - cross_up[i].dif.Value) / dates;
                //var angle = Math.Atan(Math.Abs(k2 - k1) / (1 + k1 * k2));
                var units = k.Count(p => p.date >= cross_up[i].date && p.date <= cross_up[i + 1].date);
                var deviation = new deviation
                {
                    d1 = cross_up[i].date,
                    d2 = cross_up[i + 1].date,
                    cross = units,
                    //angle = angle
                };

                if (deviation.cross > tolerance)
                    Add(deviation);
            }
        }

        static kdata cross(kdata k, Func<kdatapoint, kdatapoint, bool> cmp)
        {
            var cross = new List<kdatapoint>();
            for (var i = 0; i < k.Count - 1; ++i)
            {
                if (cmp(k[i], k[i + 1]))
                {
                    if (Math.Abs(k[i].dea.Value - k[i].dif.Value) <
                        Math.Abs(k[i + 1].dea.Value - k[i + 1].dif.Value))
                        cross.Add(k[i]);
                    else
                        cross.Add(k[i + 1]);
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
