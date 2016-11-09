using Screen.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Screen.Mixin;
using Screen.Utility;

namespace Screen.Parsers
{
    public class DataSeriesParser
    {
        static Log log = typeof(DataSeriesParser).Log();

        public static IEnumerable<StockData> Parse(string path, DateTime? date = null, string pattern = "*.txt")
        {
            var dt = date ?? DateTime.Today;

            log.Info("Search files...");
            var fileArray = Directory.GetFiles(path, pattern);

            log.Info("Read data...");
            return fileArray
                .AsParallel()
                .Select(file=>ParseFile(file, dt))
                .Where(p => p != null)
                .ToArray();
        }
        public static StockData ParseFile(string file, DateTime? date = null)
        {
            var dt = date ?? DateTime.Today;

            var name = Path.GetFileNameWithoutExtension(file).Replace("SH", "").Replace("SZ", "").Replace("#", "");
            var lines = File.ReadAllLines(file);

            var data = lines
                .Select(p =>
                {
                    var splits = p.Split(new[] { '\t', ' ',',' }, StringSplitOptions.RemoveEmptyEntries);
                    var isDate = Regex.IsMatch(splits[0], @"\d\d\d\d/\d\d/\d\d");
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

            return new StockData(name, new DataSeries(data.NetPctChange()).Section(dt));
        }
    }
}
