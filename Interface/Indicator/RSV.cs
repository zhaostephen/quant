using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class RSV : Series<double>
    {
        public RSV(kdata data, int N)
        {
            var RSVt_1 = 0d;

            for (int i = N - 1; i < data.Count; i++)
            {
                var Ct = data[i].close;
                var Ln = double.MaxValue;
                var Hn = double.MinValue;
                for(var j = i; j > i - N; --j)
                {
                    if (data[j].low < Ln)
                        Ln = data[j].low;
                    if (data[j].high > Hn)
                        Hn = data[j].high;
                }
                var RSVt = (Hn == Ln) ? RSVt_1 : ((Ct - Ln) / (Hn - Ln) * 100);

                RSVt_1 = RSVt;

                Add(new sValue<double>(data[i].date, RSVt));
            }
        }
    }
}
