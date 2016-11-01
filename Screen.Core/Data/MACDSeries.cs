using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class MACDPoint
    {
        public DateTime Date { get; set; }
        public double DIF { get; set; }
        public double DEA { get; set; }
        public double MACD { get; set; }
    }

    public class MACDSeries : List<MACDPoint>
    {
        public MACDSeries() { }

        public MACDSeries(IEnumerable<MACDPoint> collection)
            : base(collection)
        { }

        public MACDSeries Order()
        {
            return new MACDSeries(this.OrderBy(p => p.Date));
        }

        public MACDSeries OrderDescending()
        {
            return new MACDSeries(this.OrderByDescending(p => p.Date));
        }
        public MACDPoint WHICH(DateTime dt)
        {
            var v = this.SingleOrDefault(d => d.Date == dt);
            if (v == null) return null;
            return v;
        }
    }
}
