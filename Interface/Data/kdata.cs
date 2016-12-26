using Trade.Cfg;
using Trade.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using Interface.Indicator;

namespace Trade.Data
{
    public class kdata : List<kdatapoint>
    {
        public string Code { get; set; }

        public kdata(string code, IEnumerable<kdatapoint> d = null)
        {
            Code = code;
            if(d != null)
                AddRange(d);
        }

        public Series<double> close()
        {
            return series(p => p.close);
        }

        public Series<double> low()
        {
            return series(p => p.low);
        }

        public Series<double> open()
        {
            return series(p => p.open);
        }

        public Series<double> high()
        {
            return series(p => p.high);
        }

        public Series<double> volume(int units=1)
        {
            return series(p => p.volume/ units);
        }

        public Series<double> series(Func<kdatapoint, double> selector)
        {
            return new Series<double>(this.Select(p => new sValue<double> { Date = p.date, Value = selector(p) }).ToArray());
        }
    }

    public class kdatapoint
    {
        public DateTime date { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double volume { get; set; }
    }
}
