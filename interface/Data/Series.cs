using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public class Series<T> : List<sValue<T>> where T : IComparable
    {
        public Series() { }

        public Series(IEnumerable<sValue<T>> collection)
            : base(collection)
        { }

        public T this[DateTime dt]
        {
            get
            {
                var v = this.SingleOrDefault(d => d.Date == dt);
                if (v == null) return default(T);
                return v.Value;
            }
        }
    }

    public class sValue<T> where T : IComparable
    {
        public DateTime Date { get; set; }
        public T Value { get; set; }

        public sValue()
        {

        }
        public sValue(DateTime date, T value)
        {
            Date = date;
            Value = value;
        }

        public int CompareTo(sValue<T> other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
