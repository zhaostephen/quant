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

            Console.WriteLine("**********START**********");

            Console.WriteLine("Read daily");
            var daily = DataSeriesParser.Parse(@"D:\screen\Data", DateTime.Today);

            Console.WriteLine("Build weekly");
            var weekly = BuildWeeklyTimeSeries(daily, @"D:\screen\Data\week");

            Console.WriteLine("Build monthly");
            var monthly = BuildMonthlyTimeSeries(daily, @"D:\screen\Data\month");

            Console.WriteLine("Calculate...");
            var results = Analyze(daily, new DateTime(2015,5,1));

            Console.WriteLine("Save...");
            Save(results, "__results__.csv");

            Console.WriteLine("**********DONE**********");
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
            Console.WriteLine("Scan...");
            var screen = new Break3Screen();
            var result = new List<string>();
            var counter = 0;
            foreach (var pair in dataArray.AsParallel())
            {
                var screenResult = screen.Do(pair.Name, pair.Data);

                Console.WriteLine("{0}/{1}, {2} --- {3}", Interlocked.Increment(ref counter), dataArray.Count(), pair.Name, screenResult.ErrorMessage);

                if (screenResult.Good)
                {
                    result.Add(pair.Name);
                }
            }

            Console.WriteLine(string.Join(",", result));
            File.WriteAllLines("screen.txt", result);
        }
    }
}
