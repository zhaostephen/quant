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
            var arguments = Arguments.Parse(args);
            if (arguments == null) return;

            var date = arguments.Date ?? DateTime.Today;
            var stockDB = new stockDB(arguments);

            storeFundBs(stockDB);
            calcBeta(stockDB);
            screen(stockDB);
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
