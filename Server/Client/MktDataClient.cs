using Screen.Data;
using Screen.Mixin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Screen
{
    public class MktDataClient
    {
        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var file = Path.Combine(PeriodPath(period), code + ".csv");

            return QueryFile(file);
        }

        public IEnumerable<StkDataSeries> Query(IEnumerable<string> codes, PeriodEnum period = PeriodEnum.Daily)
        {
            return codes.Distinct().Select(p => Query(p, period)).ToArray();
        }

        public IEnumerable<StkDataSeries> QueryAll(PeriodEnum period = PeriodEnum.Daily)
        {
            return QueryDirectory(PeriodPath(period));
        }

        private string PeriodPath(PeriodEnum period)
        {
            switch (period)
            {
                case PeriodEnum.Daily:
                    return @"D:\screen\Data\daily";
                case PeriodEnum.Weekly:
                    return @"D:\screen\Data\week";
                case PeriodEnum.Monthly:
                    return @"D:\screen\Data\month";
                default:
                    throw new Exception("Unsupported period " + period);
            }
        }

        private IEnumerable<StkDataSeries> QueryDirectory(string path)
        {
            var fileArray = Directory.GetFiles(path, "*.txt");
            return fileArray
                .AsParallel()
                .Select(file => QueryFile(file))
                .Where(p => p != null)
                .ToArray();
        }

        private StkDataSeries QueryFile(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
            var lines = File.ReadAllLines(path);

            var data = lines
                .Select(p =>
                {
                    var splits = p.Split(new[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
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

            return new StkDataSeries(name, new DataSeries(data.NetPctChange()));
        }
    }

    public enum PeriodEnum
    {
        Daily,
        Weekly,
        Monthly
    }
}
