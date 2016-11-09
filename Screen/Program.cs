using CommandLine;
using Screen.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Screen.Mixin;
using Screen.Indicator;
using System.Threading;
using Screen.Screens;
using Screen.Data;
using ServiceStack;
using Screen.Utility;

namespace Screen
{
    class stockDB
    {
        public Security[] securities { get; private set; }
        public FundB[] fundBs { get; private set; }
        public IEnumerable<StockData> stockData { get; private set; }

        public stockDB(Arguments arguments)
        {
            securities = RefereceDataParser.ParseSecurites(@"D:\stockdata\references\沪深Ａ股.txt");
            fundBs = RefereceDataParser.ParseFundB(@"D:\stockdata\references\分级基金持股明细.txt");
            stockData = DataSeriesParser.Parse(@"D:\stockdata\timeseries", arguments.Date);
        }
    }
    class Program
    {
        static Log log = typeof(Program).Log();

        //date
        static void Main(string[] args)
        {
            //var arguments = Arguments.Parse(args);
            //if (arguments == null) return;

            //var date = arguments.Date ?? DateTime.Today;
            //var stockDB = new stockDB(arguments);

            //storeFundBs(stockDB);
            //calcBeta(stockDB);
            //screen(stockDB);

            log.Info("**********START**********");

            log.Info("Read daily");
            var daily = DataSeriesParser.Parse(@"D:\screen\Data", DateTime.Today);

            log.Info("Build weekly");
            var weekly = BuildWeeklyTimeSeries(daily, @"D:\screen\Data\week");

            log.Info("Build monthly");
            var monthly = BuildMonthlyTimeSeries(daily, @"D:\screen\Data\month");

            log.Info("stats daily");
            var statistics_daily = BuildStatistics(daily, @"D:\screen\Data\stat_daily");

            log.Info("stats weekly");
            var statistics_weekly = BuildStatistics(weekly, @"D:\screen\Data\stat_weekly");

            log.Info("stats monthly");
            var statistics_monthly = BuildStatistics(monthly, @"D:\screen\Data\stat_monthly");

            log.Info("Calculate...");
            var results = Analyze(daily, new DateTime(2015,5,1));

            log.Info("Save...");
            Save(results, "__results__.csv");

            log.Info("**********DONE**********");
        }

        private static IEnumerable<object> BuildStatistics(IEnumerable<StockData> d, string folder)
        {
            var result = d.AsParallel().Select(p =>
            {
                var close = p.Data.CloseTimeSeries();
                return new
                {
                    p.Name,
                    p.Data,
                    MACD = new MACD(p.Data),
                    MA5 = new MA(close, 5),
                    MA10 = new MA(close, 10),
                    MA20 = new MA(close, 20),
                    MA30 = new MA(close, 30),
                    MA55 = new MA(close, 55),
                    MA60 = new MA(close, 60),
                    MA120 = new MA(close, 120),
                    MA250 = new MA(close, 250)
                };
            })
            .ToArray()
            .AsParallel()
            .Select(p => new
            {
                p.Name,
                Data = p.Data.Select(p1=>
                {
                    var macd = p.MACD.WHICH(p1.Date);
                    return new
                    {
                        p1.Date,
                        DEA = macd==null ? (double?)null : macd.DEA,
                        DIF = macd == null ? (double?)null : macd.DIF,
                        MACD = macd == null ? (double?)null : macd.MACD,
                        MA5 = p.MA5.WHICH(p1.Date),
                        MA10 = p.MA10.WHICH(p1.Date),
                        MA20 = p.MA20.WHICH(p1.Date),
                        MA30 = p.MA30.WHICH(p1.Date),
                        MA55 = p.MA55.WHICH(p1.Date),
                        MA60 = p.MA60.WHICH(p1.Date),
                        MA120 = p.MA120.WHICH(p1.Date),
                        MA250 = p.MA250.WHICH(p1.Date)
                    };
                })
                .ToArray()
            })
            .ToArray();

            log.Info("Save..");
            foreach(var r in result.AsParallel())
            {
                Save(r.Data, Path.Combine(folder, r.Name + ".csv"));
            }

            return result;
        }

        private static IEnumerable<StockData> BuildMonthlyTimeSeries(IEnumerable<StockData> daily, string folder)
        {
            return BuildRangeTimeSeries(daily,
                folder,
                (d) => Tuple.Create(new DateTime(d.Year, d.Month, 1),
                                    new DateTime(d.Year, d.Month, 1).AddMonths(1).AddDays(-1)));
        }

        private static IEnumerable<StockData> BuildWeeklyTimeSeries(IEnumerable<StockData> daily, string folder)
        {
            return BuildRangeTimeSeries(daily,
                folder,
                (d) => Tuple.Create(d.AddDays(DayOfWeek.Monday - d.DayOfWeek), d.AddDays(DayOfWeek.Friday - d.DayOfWeek)));
        }

        private static IEnumerable<StockData> BuildRangeTimeSeries(IEnumerable<StockData> daily, string folder, Func<DateTime, Tuple<DateTime, DateTime>> func)
        {
            var d = daily.AsParallel().Select(p =>
            {
                var data = p.Data;
                var dateRanges = data.Select(p1 => func(p1.Date)).Distinct().ToArray();
                var r = dateRanges.Select(p1 =>
                                {
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

                Save(r, Path.Combine(folder, p.Name + ".csv"));

                var points = r.Select(p1 => new DataPoint { Close = p1.Close, Date = p1.Date, Open = p1.Open, High = p1.High, Low = p1.Low })
                              .ToArray()
                              .NetPctChange();
                var series = new DataSeries(points);

                return new StockData(p.Name, series);
            })
            .ToArray();

            return d;
        }

        private static IEnumerable<object> Analyze(IEnumerable<StockData> d, DateTime since)
        {
            var results = d.Select(p =>
            {
                var current = p.Data.Last();
                var s = p.Data.Where(p1 => p1.Date >= since);
                if (!s.Any()) return null;

                var lowest = s.Min(p1 => p1.Low);

                return new
                {
                    Name = p.Name,
                    Date = current.Date,
                    Open = current.Open,
                    Close = current.Close,
                    High = current.High,
                    Low = current.Low,
                    //PreClose = current.PreClose,
                    //NetChange = current.NetChange,
                    //PctChange = current.PctChange,
                    //HighNetChange = current.HighNetChange,
                    //HighPctChange = current.HighPctChange,
                    //LowNetChange = current.LowNetChange,
                    //LowPctChange = current.LowPctChange,
                    //OpenNetChange = current.OpenNetChange,
                    //OpenPctChange = current.OpenPctChange,
                    //HighLowNetChange = current.HighLowNetChange,
                    //HighLowPctChange = current.HighLowPctChange,
                    //AbsNetChange = current.AbsNetChange,
                    //AbsPctChange = current.AbsPctChange,
                    //AbsHighLowNetChange = current.AbsHighLowNetChange,
                    //AbsHighLowPctChange = current.AbsHighLowPctChange,
                    CloseEnum = current.CloseEnum,

                    Lowest = lowest,
                    Low2Low = Math.Truncate((current.Low / lowest - 1) * 100)
                };
            })
            .Where(p => p != null);

            var today = results.Max(p => p.Date);

            results = results
                .Where(p => p.Date == today)//exclude suspending
                .OrderBy(p => p.Low2Low);

            return results.ToArray();
        }

        private static void Save(IEnumerable<object> data, string file)
        {
            while (true)
            {
                var dir = Path.GetDirectoryName(file);
                if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
                    break;
                Directory.CreateDirectory(dir);
            }

            var text = data.ToCsv();
            File.WriteAllText(file, text, Encoding.UTF8);
        }

        static void calcBeta(stockDB stockDB)
        {

        }
        static void storeFundBs(stockDB stockDB)
        {
            var securityBs = new List<string>();
            foreach (var f in stockDB.fundBs)
            {
                foreach (var s in f.SecurityBs)
                {
                    if (!securityBs.Contains(s.代码))
                    {
                        securityBs.Add(s.代码);
                    }
                }
            }
            File.WriteAllLines("fund-B-stock-list.txt", securityBs.OrderBy(p => p));
        }

        static void screen(stockDB stockDB)
        {
            var dataArray = stockDB.stockData;
            log.Info("Scan...");
            var screen = new Break3Screen();
            var result = new List<string>();
            var counter = 0;
            foreach (var pair in dataArray.AsParallel())
            {
                var screenResult = screen.Do(pair.Name, pair.Data);

                log.Info("{0}/{1}, {2} --- {3}", Interlocked.Increment(ref counter), dataArray.Count(), pair.Name, screenResult.ErrorMessage);

                if (screenResult.Good)
                {
                    result.Add(pair.Name);
                }
            }

            log.Info(string.Join(",", result));
            File.WriteAllLines("screen.txt", result);
        }
    }
}
