using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Mixin;
using Interface.Data;

namespace Trade.Indicator
{
    public class MACD : List<macd>
    {
        public MACD(Series<double> data, int MID = 9, int SHORT = 12, int LONG = 26)
        {
            var EMA_SHORT = new EMA(data, SHORT);
            var EMA_MID = new EMA(data, MID);
            var EMA_LONG = new EMA(data, LONG);

            var DIF = (from s in EMA_SHORT
                      join l in EMA_LONG on s.Date equals l.Date
                      select new sValue<double>{ Date = s.Date, Value = s.Value - l.Value }).ToArray();
            var DEA = new EMA(new Series<double>(DIF.ToArray()), MID);
            var MACD = (from dif in DIF
                       join dea in DEA on dif.Date equals dea.Date
                       select new sValue<double> { Date = dif.Date, Value = (dif.Value - dea.Value) * 2 }).ToArray();

            var result = (from dif in DIF
                          join dea in DEA on dif.Date equals dea.Date
                          join macd in MACD on dif.Date equals macd.Date
                          select new macd { Date = dif.Date, DEA = Math.Round(dea.Value, 2), MACD = Math.Round(macd.Value, 2), DIF = Math.Round(dif.Value, 2) }).ToArray();

            AddRange(result);
        }

        public macd this[DateTime dt]
        {
            get
            {
                var v = this.SingleOrDefault(d => d.Date == dt);
                if (v == null) return default(macd);
                return v;
            }
        }

        public macd[] cross_gold()
        {
            return cross(gold: (i, next) => (i.MACD < 0 && next.MACD >= 0) ||
                                            (i.MACD == 0 && next.MACD > 0))
                .Select(p=>p.value)
                .ToArray();
        }

        public macd[] cross_dead()
        {
            return cross(dead: (i, next) => i.MACD >= 0 && next.MACD < 0)
                .Select(p => p.value)
                .ToArray(); ;
        }

        public cross<macd>[] cross()
        {
            return cross(
                gold: (i, next) => i.MACD < 0 && next.MACD >= 0,
                dead: (i, next) => i.MACD >= 0 && next.MACD < 0);
        }

        public static implicit operator macd(MACD o)
        {
            return o.Any() ? o.Last() : null;
        }

        private cross<macd>[] cross(Func<macd, macd, bool> gold = null, Func<macd, macd, bool> dead = null)
        {
            var k = this;
            var cross = new List<cross<macd>>();
            for (var i = 1; i < k.Count; ++i)
            {
                if (gold != null && gold(k[i - 1], k[i]))
                {
                    if (Math.Abs(k[i - 1].DEA - k[i - 1].DIF) <
                        Math.Abs(k[i].DEA - k[i].DIF))
                        cross.Add(new cross<macd>(k[i - 1], crosstype.gold));
                    else
                        cross.Add(new cross<macd>(k[i], crosstype.gold));
                }
                else if (dead != null && dead(k[i - 1], k[i]))
                {
                    if (Math.Abs(k[i - 1].DEA - k[i - 1].DIF) <
                        Math.Abs(k[i].DEA - k[i].DIF))
                        cross.Add(new cross<macd>(k[i - 1], crosstype.dead));
                    else
                        cross.Add(new cross<macd>(k[i], crosstype.dead));
                }
            }
            return cross.ToArray();
        }
    }
}
