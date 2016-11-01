using Screen.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Screen.Mixin;

namespace Screen.Parsers
{
    public class DataSeriesParser
    {
        public static IEnumerable<StockData> Parse(string path, DateTime? date = null, string pattern = "*.txt")
        {
            var dt = date ?? DateTime.Today;

            Console.WriteLine("Search files...");
            var fileArray = Directory.GetFiles(path, pattern);

            Console.WriteLine("Read data...");
            return fileArray
                .Select(file=>DataSeriesParser.ParseFile(file, dt))
                .Where(p => p != null)
                .ToArray();
        }
        public static StockData ParseFile(string file, DateTime? date = null)
        {
            var dt = date ?? DateTime.Today;

            var name = Path.GetFileNameWithoutExtension(file).Replace("SH", "").Replace("SZ", "");
            var lines = File.ReadAllLines(file);

            var data = lines
                .Select(p =>
                {
                    var splits = p.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var isDate = Regex.IsMatch(splits[0], @"\d\d/\d\d/\d\d\d\d");
                    if (!isDate) return null;

                    return new DataPoint
                    {
                        Date = splits[0].Date(),
                        Open = splits[1].Double(),
                        High = splits[2].Double(),
                        Low = splits[3].Double(),
                        Close = splits[4].Double()
                    };
                })
                .Where(p => p != null && p.Open > 0d && p.Close > 0d)
                .OrderBy(p => p.Date)
                .ToArray();

            if (!data.Any())
                return null;

            for (var i = 1; i < data.Length; ++i)
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
            
            return new StockData(name, new DataSeries(data).Section(dt));
        }
    }
}
