using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screen.Mixin;

namespace Screen.Indicator
{
    public class CCI : TimeSeries<double>
    {
        public CCI(DataSeries data, int N)
        {
            for (int i = N - 1; i < data.Count; i++)
            {
                var point = data[i];
                var section = data.Skip((i + 1) - N).Take(N);
                if (section.Count() < N)
                    break;

                var TYP = (point.Close + point.High + point.Low) / 3;
                var MA = section.Select(d => (d.Close + d.High + d.Low) / 3).MA();
                var AVEDEV = section.Select(d => (d.Close + d.High + d.Low) / 3).AVEDEV();
                var CCI = (TYP - MA) / (0.015 * AVEDEV);

                //TYP:=(HIGH+LOW+CLOSE)/3;
                //CCI:(TYP-MA(TYP,N))/(0.015*AVEDEV(TYP,N));

                this.Add(new TimePoint<double>
                {
                    Date = point.Date,
                    Value = Math.Round(CCI, 2),
                });
            }
        }
    }
}
