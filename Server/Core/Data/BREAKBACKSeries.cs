using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class BREAKBACKPoint
    {
        public BREAKPoint Break { get; set; }
        public TimePoint<double> Bottom { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

    public class BREAKBACKSeries : List<BREAKBACKPoint>
    {
        public BREAKBACKSeries() { }

        public BREAKBACKSeries(IEnumerable<BREAKBACKPoint> collection)
            : base(collection)
        { }

        public BREAKBACKSeries Order()
        {
            return new BREAKBACKSeries(this.OrderBy(p => p.Date));
        }

        public BREAKBACKSeries OrderDescending()
        {
            return new BREAKBACKSeries(this.OrderByDescending(p => p.Date));
        }

        public BREAKBACKPoint WHICH(DateTime dt)
        {
            var v = this.SingleOrDefault(d => d.Date == dt);
            if (v == null) return null;
            return v;
        }
    }
}
