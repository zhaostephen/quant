using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class JAXPoint
    {
        public DateTime Date { get; set; }
        public double JAX { get; set; }
        public double J { get; set; }
        public double A { get; set; }
        public double X { get; set; }

        public bool upward { get { return A > JAX && J == 0d && X == 0d; } }
    }

    public class JAXSeries : List<JAXPoint>
    {
        public JAXSeries() { }

        public JAXSeries(IEnumerable<JAXPoint> collection)
            : base(collection)
        { }

        public JAXSeries Order()
        {
            return new JAXSeries(this.OrderBy(p => p.Date));
        }

        public JAXSeries OrderDescending()
        {
            return new JAXSeries(this.OrderByDescending(p => p.Date));
        }

        public TimeSeries<double> TimeSeries(Func<JAXPoint, double> selector)
        {
            return new TimeSeries<double>(this.Select(p => new TimePoint<double> { Date = p.Date, Value = selector(p) }).ToArray());
        }

        public TimeSeries<double> A()
        {
            return TimeSeries(p => p.A);
        }

        public TimeSeries<double> J()
        {
            return TimeSeries(p => p.J);
        }

        public TimeSeries<double> X()
        {
            return TimeSeries(p => p.X);
        }

        public TimeSeries<double> JAX()
        {
            return TimeSeries(p => p.JAX);
        }
        public JAXPoint WHICH(DateTime dt)
        {
            var v = this.SingleOrDefault(d => d.Date == dt);
            if (v == null) return null;
            return v;
        }
    }
}
