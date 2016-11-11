using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Indicator
{
    public class PEAK : TimeSeries<double>
    {
        public PEAK(TimeSeries<double> data)
        {
            for (int i = 1; i < data.Count - 1; i++)
            {
                var cur = data[i];
                var prev = data[i - 1];
                var next = data[i + 1];
                if (cur.Value > prev.Value && cur.Value > next.Value)
                {
                    this.Add(new TimePoint<double>
                    {
                        Date = cur.Date,
                        Value = cur.Value,
                    });
                }
            }
        }
    }
}
