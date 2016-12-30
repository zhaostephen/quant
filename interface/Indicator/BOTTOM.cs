using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Indicator;

namespace Interface.Indicator
{
    public class BOTTOM : Series<double>
    {
        public BOTTOM(kdata data)
        {
            var macd = new MACD(data.close());
            var lows = new PEAK(data, PEAK_TYPE.low).Select(p=>new { p.Date, p.Value, type = PEAK_TYPE.low }).ToArray();
            var highs = new PEAK(data, PEAK_TYPE.high).Select(p => new { p.Date, p.Value, type = PEAK_TYPE.high }).ToArray();
            var peaks = lows.Concat(highs).OrderBy(p => p.Date).ToArray();

            for (var i = 0; i < peaks.Length - 3; ++i)
            {
                var a = peaks[i];
                var b = peaks[i + 1];
                var c = peaks[i + 2];
                var d = peaks[i + 3];

                if(a.type == PEAK_TYPE.low && 
                    b.type == PEAK_TYPE.high && 
                    c.type == PEAK_TYPE.low && 
                    d.type == PEAK_TYPE.high && 
                    b.Value > a.Value && 
                    c.Value < b.Value &&
                    c.Value >= a.Value && 
                    d.Value > b.Value)
                {
                    var next = macd.Where(p => p.Date > d.Date).ToArray();
                    for(var k = 2; k < next.Length; ++k)
                    {
                        if (next[k].DIF < 0 &&
                            next[k - 1].DIF < 0 &&
                            next[k - 2].DIF < 0 &&
                            next[k].DIF > next[k - 1].DIF &&
                            next[k].DIF > next[k - 2].DIF)
                        {
                            var v = data.FirstOrDefault(p=>p.date == next[k].Date);
                            if (v != null)
                            {
                                if (!this.Any(p => p.Date == next[k].Date))
                                {
                                    Add(next[k].Date, v.high);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
