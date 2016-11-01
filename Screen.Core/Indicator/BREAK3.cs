using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Indicator
{
    public class BREAK3 : BREAKSeries
    {
        public BREAK3(TimeSeries<double> data, double N = 0.02)
        {
            var peaks = new PEAK(data);

            var peakdate = DateTime.MinValue;
            for (int i = 2; i < data.Count; i++)
            {
                var cur = data[i];
                var next1 = data[i - 1];
                var next2 = data[i - 2];

                //找到前期高点（没有被突破的）
                var peak = peaks.LastOrDefault(p => p.Date < cur.Date && p.Date > peakdate);
                if (peak == null)
                    continue;

                var strongbreak = (cur.Value > peak.Value && cur.Value >= (1 + N) * peak.Value);

                if (strongbreak)
                {
                    peakdate = cur.Date;
                    this.Add(new BREAKPoint
                    {
                        Date = cur.Date,
                        Value = cur.Value,
                        Peak = peak
                    });
                }
            }
        }
    }
}
