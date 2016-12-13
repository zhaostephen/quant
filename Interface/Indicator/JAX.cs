using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class JAX : List<jax>
    {
        public JAX(kdata data, int N, int M)
        {
            var MA = new MA(data.close(), N);
            var MA3 = new MA(data.TimeSeries(d => (2 * d.close + d.low + d.high) / 4), M);
            var func = new Func<kdatapoint, double, double>((d, salt) => d.close/salt * (2 * d.close + d.low + d.high) / 4);
            var dmaPrevValue = 0d;

            for (int i = M - 1; i < data.Count; i++)
            {
                var curr = data[i];

                if (MA[curr.date] == 0d) continue;

                var AA = Math.Abs((2 * curr.close + curr.high + curr.low) / 4 - MA[curr.date]) / MA[curr.date];
                var dmaValue = (2 * curr.close + curr.low + curr.high) / 4;
                var jax = AA * dmaValue + (1 - AA) * dmaPrevValue;
                dmaPrevValue = jax;

                var MA1 = data.Skip(i + 1 - M).Take(M).Select(p => func(p, jax)).Average();
                var MAAA = ((MA1 - jax) / jax) / 3.0d;
                var TMP = MA1 - MAAA * MA1;
                var J = TMP <= jax ? jax : 0d;
                var A = TMP;
                var X = TMP <= jax ? TMP : 0d;

                Add(new jax { Date = curr.date, JAX = Math.Round(jax, 2), J = Math.Round(J, 2), A = Math.Round(A, 2), X = Math.Round(X, 2) });
            }
        }
    }
}
