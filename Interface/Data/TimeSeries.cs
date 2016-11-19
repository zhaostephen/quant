using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public class TimePoint<T> where T:IComparable
    {
        public DateTime Date { get; set; }
        public T Value { get; set; }

        public TimePoint()
        {

        }
        public TimePoint(DateTime date, T value)
        {
            Date = date;
            Value = value;
        }

        public int CompareTo(TimePoint<T> other)
        {
            return Value.CompareTo(other.Value);
        }
    }

    public class TimeSeries<T> : List<TimePoint<T>> where T : IComparable
    {
        public TimeSeries() { }

        public TimeSeries(IEnumerable<TimePoint<T>> collection)
            : base(collection)
        { }

        public TimeSeries<T> Order()
        {
            return new TimeSeries<T>(this.OrderBy(p => p.Date));
        }

        public TimeSeries<T> OrderDescending()
        {
            return new TimeSeries<T>(this.OrderByDescending(p => p.Date));
        }

        public TimeSeries<T> Section(DateTime? endInclusive = null, DateTime? startInclusive = null)
        {
            startInclusive = startInclusive ?? DateTime.MinValue;
            endInclusive = endInclusive ?? DateTime.MaxValue;

            var data =
                this.Where(p => p.Date >= startInclusive.Value && p.Date <= endInclusive.Value).ToArray();

            return new TimeSeries<T>(data);
        }

        public TimeSeries<T> Section(DateTime endInclusive, int N)
        {
            var data =
                this.Where(p => p.Date <= endInclusive).Take(N).ToArray();

            return new TimeSeries<T>(data);
        }

        public T WHICH(DateTime dt)
        {
            var  v= this.SingleOrDefault(d => d.Date == dt);
            if (v == null) return default(T);
            return v.Value;
        }
    }
}
