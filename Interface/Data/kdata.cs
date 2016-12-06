using Trade.Cfg;
using Trade.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public kdata complete()
        {
            var close = CloseTimeSeries();

            var MACD = new MACD(this).ToDictionary(p => p.Date, p => p);
            var MA5 = new MA(close, 5).ToDictionary(p => p.Date, p => p);
            var MA10 = new MA(close, 10).ToDictionary(p => p.Date, p => p);
            var MA20 = new MA(close, 20).ToDictionary(p => p.Date, p => p);
            var MA30 = new MA(close, 30).ToDictionary(p => p.Date, p => p);
            var MA55 = new MA(close, 55).ToDictionary(p => p.Date, p => p);
            var MA60 = new MA(close, 60).ToDictionary(p => p.Date, p => p);
            var MA120 = new MA(close, 120).ToDictionary(p => p.Date, p => p);
            var MA250 = new MA(close, 250).ToDictionary(p => p.Date, p => p);
            var PEAK_L = new PEAK(this, PEAK_TYPE.low).ToDictionary(p => p.Date, p => p);
            var PEAK_H = new PEAK(this, PEAK_TYPE.high).ToDictionary(p => p.Date, p => p);

            foreach (var p in this)
            {
                var date = p.date;
                var macd = MACD.ContainsKey(date) ? MACD[date] : null;

                p.dea = macd != null ? macd.DEA : (double?)null;
                p.dif = macd != null ? macd.DIF : (double?)null;
                p.macd = macd != null ? macd.MACD : (double?)null;
                p.ma5 = MA5.ContainsKey(date) ? MA5[date].Value : (double?)null;
                p.ma10 = MA10.ContainsKey(date) ? MA10[date].Value : (double?)null;
                p.ma20 = MA20.ContainsKey(date) ? MA20[date].Value : (double?)null;
                p.ma30 = MA30.ContainsKey(date) ? MA30[date].Value : (double?)null;
                p.ma55 = MA55.ContainsKey(date) ? MA55[date].Value : (double?)null;
                p.ma60 = MA60.ContainsKey(date) ? MA60[date].Value : (double?)null;
                p.ma120 = MA120.ContainsKey(date) ? MA120[date].Value : (double?)null;
                p.ma250 = MA250.ContainsKey(date) ? MA250[date].Value : (double?)null;
                p.peak_l = PEAK_L.ContainsKey(date) ? PEAK_L[date].Value : (double?)null;
                p.peak_h = PEAK_H.ContainsKey(date) ? PEAK_H[date].Value : (double?)null;
            }
            return this;
        }

        public TimeSeries<double> TimeSeries(Func<kdatapoint, double> selector)
        {
            return new TimeSeries<double>(this.Select(p => new TimePoint<double> { Date = p.date, Value = selector(p) }).ToArray());
        }

        public TimeSeries<double> CloseTimeSeries()
        {
            return TimeSeries(p => p.close);
        }

        public TimeSeries<double> LowTimeSeries()
        {
            return TimeSeries(p => p.low);
        }

        public TimeSeries<double> OpenTimeSeries()
        {
            return TimeSeries(p => p.open);
        }

        public TimeSeries<double> HighTimeSeries()
        {
            return TimeSeries(p => p.high);
        }
    }

    public class kdatapoint
    {
        public DateTime date { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double? macd { get; set; }
        public double? dif { get; set; }
        public double? dea { get; set; }
        public double? ma5 { get; set; }
        public double? ma10 { get; set; }
        public double? ma20 { get; set; }
        public double? ma30 { get; set; }
        public double? ma55 { get; set; }
        public double? ma60 { get; set; }
        public double? ma120 { get; set; }
        public double? ma250 { get; set; }
        public double? peak_l { get; set; }
        public double? peak_h { get; set; }
    }
}
