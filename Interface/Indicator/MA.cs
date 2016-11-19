using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class MA : TimeSeries<double>
    {
        public MA(TimeSeries<double> data, int N)
        {
            for (int i = N - 1; i < data.Count; i++)
            {
                var point = data[i];
                var section = data.Skip((i + 1) - N).Take(N);
                if (section.Count() < N)
                    break;

                var value = Math.Round(section.Select(p => p.Value).Average(), 2);
                if (value == 0d)
                    continue;

                this.Add(new TimePoint<double>
                {
                    Date = point.Date,
                    Value = value,
                });
            }
        }
    }
}
