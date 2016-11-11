using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class DataSeries : List<DataPoint>
    {
        public DataSeries() { }

        public DataSeries(IEnumerable<DataPoint> collection)
            : base(collection)
        { }

        public TimeSeries<double> TimeSeries(Func<DataPoint, double> selector)
        {
            return new TimeSeries<double>(this.Select(p => new TimePoint<double> { Date = p.Date, Value = selector(p) }).ToArray());
        }

        public TimeSeries<double> CloseTimeSeries()
        {
            return TimeSeries(p => p.Close);
        }

        public TimeSeries<double> LowTimeSeries()
        {
            return TimeSeries(p => p.Low);
        }

        public TimeSeries<double> OpenTimeSeries()
        {
            return TimeSeries(p => p.Open);
        }

        public TimeSeries<double> HighTimeSeries()
        {
            return TimeSeries(p => p.High);
        }

        public DataSeries Order()
        {
            return new DataSeries(this.OrderBy(p => p.Date));
        }

        public DataSeries OrderDescending()
        {
            return new DataSeries(this.OrderByDescending(p => p.Date));
        }

        public DataSeries Section(DateTime? endInclusive = null, DateTime? startInclusive = null)
        {
            startInclusive = startInclusive ?? DateTime.MinValue;
            endInclusive = endInclusive ?? DateTime.MaxValue;

            var data =
                this.Where(p => p.Date >= startInclusive.Value && p.Date <= endInclusive.Value).ToArray();

            return new DataSeries(data);
        }
    }
}
