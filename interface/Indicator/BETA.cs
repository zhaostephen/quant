using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Indicator;

namespace Interface.Indicator
{
    public class BETA
    {
        public double alpha { get; private set; }
        public double beta { get; private set; }

        public BETA(kdata data, kdata index)
        {
            var q = from i in index
                    join s in data on i.date equals s.date
                    select new { stock = s.close, index = i.close };

            var ret = SimpleRegression.Fit(
                pct(q.Select(p=>p.index).ToArray()),
                pct(q.Select(p => p.stock).ToArray()));

            alpha = ret.Item1;
            beta = ret.Item2;
        }

        static double[] pct(double[] d)
        {
            if (d.Length < 2) return new double[0];

            var pct = new double[d.Length - 1];
            for(var i = 1; i < d.Length; ++i)
            {
                pct[i - 1] = (d[i] - d[i - 1]) / d[i-1];
            }

            return pct;
        }
    }
}
