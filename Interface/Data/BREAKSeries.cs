using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public class BREAKPoint
    {
        public TimePoint<double> Peak { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

    public class BREAKSeries : List<BREAKPoint>
    {
        public BREAKSeries() { }

        public BREAKSeries(IEnumerable<BREAKPoint> collection)
            : base(collection)
        { }

        public BREAKSeries Order()
        {
            return new BREAKSeries(this.OrderBy(p => p.Date));
        }

        public BREAKSeries OrderDescending()
        {
            return new BREAKSeries(this.OrderByDescending(p => p.Date));
        }

        public BREAKPoint WHICH(DateTime dt)
        {
            var v = this.SingleOrDefault(d => d.Date == dt);
            if (v == null) return null;
            return v;
        }
    }
}
