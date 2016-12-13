using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Mixin;

namespace Trade.Indicator
{
    public class STDP : Series<double>
    {
        public STDP(Series<double> data, int N)
        {
            for (int i = N - 1; i < data.Count; i++)
            {
                var value = 0d;
                for (var j = 0; j < N; ++j)
                    value += data[i - j].Value;
                var average = value / N;

                var sum = 0d;
                for (var j = 0; j < N; ++j)
                    sum += Math.Pow(data[i - j].Value - average, 2);
                var sqrt = Math.Sqrt(sum / N);

                Add(new sValue<double>(data[i].Date, sqrt));
            }
        }
    }
}
