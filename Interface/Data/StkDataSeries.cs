using Trade.Cfg;
using Trade.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trade.Data
{
    public class StkDataSeries : DataSeries
    {
        public string Code { get; set; }

        public StkDataSeries(string code, DataSeries d)
        {
            Code = code;
            AddRange(d);
        }
        public StkDataSeries(string code, IEnumerable<DataPoint> d)
        {
            Code = code;
            AddRange(d);
        }

        public StkDataSeries complete()
        {
            completeChange();
            completeIndicators();
            return this;
        }
        private void completeIndicators()
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
        private void completeChange()
        {
            var data = this;
            for (var i = 1; i < data.Count; ++i)
            {
                var preClose = data[i - 1].Close;
                data[i].PreClose = preClose;

                data[i].NetChange = data[i].Close - preClose;
                data[i].PctChange = Math.Round(((data[i].Close - preClose) / preClose) * 100, 2);

                data[i].LowNetChange = data[i].Low - preClose;
                data[i].LowPctChange = Math.Round(((data[i].Low - preClose) / preClose) * 100, 2);

                data[i].HighNetChange = data[i].High - preClose;
                data[i].HighPctChange = Math.Round(((data[i].High - preClose) / preClose) * 100, 2);

                data[i].OpenNetChange = data[i].Open - preClose;
                data[i].OpenPctChange = Math.Round(((data[i].Open - preClose) / preClose) * 100, 2);

                data[i].HighLowNetChange = data[i].High - data[i].Low;
                data[i].HighLowPctChange = Math.Round(((data[i].High - data[i].Low) / preClose) * 100, 2);
            }
        }

        public StkDataSeries Make(PeriodEnum basePeriod, PeriodEnum followingPeriod)
        {
            if(basePeriod == PeriodEnum.Daily)
            {
                switch (followingPeriod)
                {
                    case PeriodEnum.Daily:
                        return this.complete();
                    case PeriodEnum.Weekly:
                        return Roll(this,(d) => Tuple.Create(d.AddDays(DayOfWeek.Monday - d.DayOfWeek), d.AddDays(DayOfWeek.Friday - d.DayOfWeek))).complete();
                    case PeriodEnum.Monthly:
                        return Roll(this, (d) => Tuple.Create(
                            new DateTime(d.Year, d.Month, 1),
                            new DateTime(d.Year, d.Month, 1).AddMonths(1).AddDays(-1))).complete();
                }
            }
            else if (basePeriod == PeriodEnum.Min5)
            {
                switch (followingPeriod)
                {
                    case PeriodEnum.Min5:
                        return new StkDataSeries(this.Code,this.Take(720).ToArray()).complete();
                    case PeriodEnum.Min15:
                        return RollMinutes(15, 480 * 15 / 5).complete();
                    case PeriodEnum.Min30:
                        return RollMinutes(30, 240 * 30 / 5).complete();
                    case PeriodEnum.Min60:
                        return RollMinutes(60, 120 * 60 / 5).complete();
                }
            }

            throw new Exception("Unsupported " + basePeriod + " ==> " + followingPeriod);
        }

        private StkDataSeries RollMinutes(int minutes, int howMany)
        {
            var am = new DateTime(2010, 1, 1, 9, 30, 0);
            var pm = new DateTime(2010, 1, 1, 13, 00, 0);
            var count = 120 / minutes;
            var amRange = Enumerable.Range(1, count)
                .Select(p => Tuple.Create(am.AddMinutes((p - 1) * minutes), am.AddMinutes(minutes * p)));
            var pmRange = Enumerable.Range(1, count)
                .Select(p => Tuple.Create(pm.AddMinutes((p - 1) * minutes), pm.AddMinutes(minutes * p)));
            var range = amRange.Concat(pmRange).ToArray();

            var skip = Math.Max(Count - howMany, 0);
            var data = new StkDataSeries(Code, this.Skip(skip).ToArray());
            return Roll(data, (d) =>
            {
                var time = new DateTime(2010, 1, 1, d.Hour, d.Minute, 0);

                var r = range.SingleOrDefault(p => time > p.Item1 && time <= p.Item2);

                if (r == null) return null;

                return Tuple.Create(
                    new DateTime(d.Year, d.Month, d.Day, r.Item1.Hour, r.Item1.Minute, r.Item1.Second).AddSeconds(1),
                    new DateTime(d.Year, d.Month, d.Day, r.Item2.Hour, r.Item2.Minute, r.Item2.Second));
            });
        }

        private static StkDataSeries Roll(StkDataSeries data,Func<DateTime, Tuple<DateTime, DateTime>> func)
        {
            var dateRanges = data.Select(p1 => func(p1.Date)).Distinct().ToArray();
            var r = dateRanges.Select(p1 =>
            {
                if (p1 == null) return null;
                var range = data.Section(p1.Item2, p1.Item1);
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
                          .ToArray();

            return new StkDataSeries(data.Code, points);
        }
    }
}
