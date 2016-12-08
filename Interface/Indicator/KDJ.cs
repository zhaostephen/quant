using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class KDJ : List<kdj>
    {
        public KDJ(kdata data, int N = 9, int M1 = 3, int M2 = 3)
        {
            var rsv = new RSV(data, N);
            var K = new SMA(rsv, M1, 50);
            var D = new SMA(K, M2, 50);
            var J = (from k in K
                     join d in D on k.Date equals d.Date
                     select new TimePoint<double>(k.Date, 3 * k.Value - 2 * d.Value)).ToArray();
            var p = from k in K
                    join d in D on k.Date equals d.Date
                    join j in J on k.Date equals j.Date
                    select new kdj { Date = k.Date, K = Math.Round(k.Value, 2), D = Math.Round(d.Value, 2), J = Math.Round(j.Value, 2) };

            AddRange(p.ToArray());
        }

        public IEnumerable<kdj> cross_up()
        {
            var kdj = this;
            for (var i = 1; i < kdj.Count; ++i)
            {
                if (!kdj[i - 1].up() && kdj[i].up())
                    yield return kdj[i - 1].width() < kdj[i].width() ? kdj[i - 1] : kdj[i];
            }
        }

        public IEnumerable<kdj> cross_down()
        {
            var kdj = this;
            for (var i = 1; i < kdj.Count; ++i)
            {
                if (kdj[i - 1].up() && !kdj[i].up())
                    yield return kdj[i - 1].width() < kdj[i].width() ? kdj[i - 1] : kdj[i];
            }
        }
    }

    public class kdj
    {
        public DateTime Date { get; set; }
        public double K { get; set; }
        public double D { get; set; }
        public double J { get; set; }

        public bool up() { return J > K && K > D; }
        public double width() { return Math.Abs(J - D); }
    }
}
