using Screen.Cfg;
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

            var MACD = new MACD(this).ToDictionary(p => p.Date, p => p);
            var MA5 = new MA(close, 5).ToDictionary(p => p.Date, p => p);
            var MA10 = new MA(close, 10).ToDictionary(p => p.Date, p => p);
            var MA20 = new MA(close, 20).ToDictionary(p => p.Date, p => p);
            var MA30 = new MA(close, 30).ToDictionary(p => p.Date, p => p);
            var MA55 = new MA(close, 55).ToDictionary(p => p.Date, p => p);
            var MA60 = new MA(close, 60).ToDictionary(p => p.Date, p => p);
            var MA120 = new MA(close, 120).ToDictionary(p => p.Date, p => p);
            var MA250 = new MA(close, 250).ToDictionary(p => p.Date, p => p);

            foreach (var p in this)
            {
                var date = p.Date;
                var macd = MACD.ContainsKey(date) ? MACD[date] : null;

                p.DEA = macd != null ? macd.DEA : (double?)null;
                p.DIF = macd != null ? macd.DIF : (double?)null;
                p.MACD = macd != null ? macd.MACD : (double?)null;
                p.MA5 = MA5.ContainsKey(date) ? MA5[date].Value : (double?)null;
                p.MA10 = MA10.ContainsKey(date) ? MA10[date].Value : (double?)null;
                p.MA20 = MA20.ContainsKey(date) ? MA20[date].Value : (double?)null;
                p.MA30 = MA30.ContainsKey(date) ? MA30[date].Value : (double?)null;
                p.MA55 = MA55.ContainsKey(date) ? MA55[date].Value : (double?)null;
                p.MA60 = MA60.ContainsKey(date) ? MA60[date].Value : (double?)null;
                p.MA120 = MA120.ContainsKey(date) ? MA120[date].Value : (double?)null;
                p.MA250 = MA250.ContainsKey(date) ? MA250[date].Value : (double?)null;
            }
        }

        public StkDataSeries Make(PeriodEnum basePeriod, PeriodEnum followingPeriod)
        {
            if (basePeriod == followingPeriod) return this;

            if(basePeriod == PeriodEnum.Daily)
            {
                switch (followingPeriod)
                {
                    case PeriodEnum.Weekly:
                        return Roll((d) => Tuple.Create(d.AddDays(DayOfWeek.Monday - d.DayOfWeek), d.AddDays(DayOfWeek.Friday - d.DayOfWeek)));
                    case PeriodEnum.Monthly:
                        return Roll((d) => Tuple.Create(
                            new DateTime(d.Year, d.Month, 1),
                            new DateTime(d.Year, d.Month, 1).AddMonths(1).AddDays(-1)));
                }
            }
            else if(basePeriod == PeriodEnum.Min_5)
            {
                switch (followingPeriod)
                {
                    case PeriodEnum.Min_15:
                        return RollMinutes(15);
                    case PeriodEnum.Min_30:
                        return RollMinutes(30);
                    case PeriodEnum.Min_60:
                        return RollMinutes(60);
                }
            }

            throw new Exception("Unsupported " + basePeriod + " ==> " + followingPeriod);
        }

        private StkDataSeries RollMinutes(int minutes)
        {
            var am = new DateTime(2010, 1, 1, 9, 30, 0);
            var pm = new DateTime(2010, 1, 1, 13, 00, 0);
            var count = 120 / minutes;
            var amRange = Enumerable.Range(1, count)
                .Select(p => Tuple.Create(am.AddMinutes((p - 1) * minutes), am.AddMinutes(minutes * p)));
            var pmRange = Enumerable.Range(1, count)
                .Select(p => Tuple.Create(pm.AddMinutes((p - 1) * minutes), pm.AddMinutes(minutes * p)));
            var range = amRange.Concat(pmRange).ToArray();

            return Roll((d) =>
            {
                var time = new DateTime(2010, 1, 1, d.Hour, d.Minute, 0);

                var r = range.SingleOrDefault(p => time > p.Item1 && time <= p.Item2);

                if (r == null) return null;

                return Tuple.Create(
                    new DateTime(d.Year, d.Month, d.Day, r.Item1.Hour, r.Item1.Minute, r.Item1.Second).AddSeconds(1),
                    new DateTime(d.Year, d.Month, d.Day, r.Item2.Hour, r.Item2.Minute, r.Item2.Second));
            });
        }

        private StkDataSeries Roll(Func<DateTime, Tuple<DateTime, DateTime>> func)
        {
            var dateRanges = this.Select(p1 => func(p1.Date)).Distinct().ToArray();
            var r = dateRanges.Select(p1 =>
            {
                if (p1 == null) return null;
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
