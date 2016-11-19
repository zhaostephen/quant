using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class BREAK_BACK : BREAKBACKSeries
    {
        public BREAK_BACK(TimeSeries<double> data)
        {
            if (!data.Any()) return;

            var date = data.Max(p=>p.Date);
            var peaks = new PEAK(data);
            var breaks = new BREAK(data);
            var bottoms = new BOTTOM(data);

            foreach(var brk in breaks)
            {
                //寻找回踩点
                var nextNextPeak = peaks.Where(p => p.Date > brk.Date).Skip(1).Take(1).FirstOrDefault();
                var nextPeakDate = nextNextPeak != null ? nextNextPeak.Date : date;
                var nextBottom = bottoms.FirstOrDefault(p => p.Date > brk.Date && p.Date <= nextPeakDate);
                if (nextBottom == null)
                    continue;

                TimePoint<double> buy = null;

                var N = 3;
                var nextBottomN = data.Where(p => p.Date > nextBottom.Date && p.Date <= nextPeakDate).Take(N);
                var up3pctoneday = nextBottomN.FirstOrDefault(p => (p.Value / brk.Peak.Value) - 1 >= 0.03d);
                if (up3pctoneday != null)
                    buy = up3pctoneday;
                else
                {
                    var up3days = nextBottomN.Sum(p => (p.Value / brk.Peak.Value) - 1) >= 0.03d;
                    if (up3days)
                    {
                        buy = nextBottomN.Last();
                    }
                }
                if(buy !=null)
                {
                    //between break and bottom, there would be a peak
                    var peakBetween = peaks.Where(p => p.Date >= brk.Date && p.Date <= nextBottom.Date).FirstOrDefault();
                    if (peakBetween != null && buy.Value >= peakBetween.Value)
                        buy = null;
                }
                if (buy != null)
                {
                    this.Add(new BREAKBACKPoint
                    {
                        Date = buy.Date,
                        Value = buy.Value,
                        Bottom = nextBottom,
                        Break = brk
                    });
                }
            }
        }
    }
}
