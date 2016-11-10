using Screen.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Screen.Data
{
    public class StkDataSeries : DataSeries
    {
        public string Code { get; set; }

        public StkDataSeries(string code, DataSeries d)
        {
            Code = code;
            AddRange(d);
            Indicators();
        }

        private void Indicators()
        {
            var close = CloseTimeSeries();

            var MACD = new MACD(this);
            var MA5 = new MA(close, 5);
            var MA10 = new MA(close, 10);
            var MA20 = new MA(close, 20);
            var MA30 = new MA(close, 30);
            var MA55 = new MA(close, 55);
            var MA60 = new MA(close, 60);
            var MA120 = new MA(close, 120);
            var MA250 = new MA(close, 250);

            foreach(var p in this)
            {
                var date = p.Date;
                var macd = MACD.WHICH(date);

                p.DEA = macd == null ? (double?)null : macd.DEA;
                p.DIF = macd == null ? (double?)null : macd.DIF;
                p.MACD = macd == null ? (double?)null : macd.MACD;
                p.MA5 = MA5.WHICH(date);
                p.MA10 = MA10.WHICH(date);
                p.MA20 = MA20.WHICH(date);
                p.MA30 = MA30.WHICH(date);
                p.MA55 = MA55.WHICH(date);
                p.MA60 = MA60.WHICH(date);
                p.MA120 = MA120.WHICH(date);
                p.MA250 = MA250.WHICH(date);
            }
        }

        public StkDataSeries MakeMonth()
        {
            return Roll((d) => Tuple.Create(
                new DateTime(d.Year, d.Month, 1),
                new DateTime(d.Year, d.Month, 1).AddMonths(1).AddDays(-1)));
        }

        public StkDataSeries MakeWeek()
        {
            return Roll((d) => Tuple.Create(d.AddDays(DayOfWeek.Monday - d.DayOfWeek), d.AddDays(DayOfWeek.Friday - d.DayOfWeek)));
        }

        public StkDataSeries Roll(Func<DateTime, Tuple<DateTime, DateTime>> func)
        {
            var dateRanges = this.Select(p1 => func(p1.Date)).Distinct().ToArray();
            var r = dateRanges.Select(p1 =>
            {
                var range = Section(p1.Item2, p1.Item1);
                if (!range.Any()) return null;

                return new
                {
                    Date = range.Last().Date,
                    Open = range.First().Open,
                    Close = range.Last().Close,
                    High = range.Max(p2 => p2.High),
                    Low = range.Max(p2 => p2.Low)
                };
            })
            .Where(p1 => p1 != null)
            .ToArray();

            var points = r.Select(p1 => new DataPoint { Close = p1.Close, Date = p1.Date, Open = p1.Open, High = p1.High, Low = p1.Low })
                          .ToArray()
                          .NetPctChange();
            var series = new DataSeries(points);

            return new StkDataSeries(Code, series);
        }
    }
}
