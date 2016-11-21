using Trade.Cfg;
using Trade.Data;
using Trade.Mixin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trade
{
    public class MktDataClient
    {
        public IEnumerable<Fundamental> QueryFundamentals()
        {
            var path = Configuration.level1.fundamental;
            if (!File.Exists(path))
                return Enumerable.Empty<Fundamental>();

            var lines = File.ReadAllLines(path);
            if(!lines.Any())
                return Enumerable.Empty<Fundamental>();

            var columns = lines[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return lines
                .Skip(1)
                .Select(p =>
                {
                    var splits = p.Split(new[] {',' }, StringSplitOptions.RemoveEmptyEntries);
                    var f = new Fundamental();
                    for(var i  = 0; i < columns.Length; ++i)
                    {
                        var column = columns[i];
                        f.SetPropertyValue(column, splits[i]);
                    }
                    return f;
                })
                .ToArray();
        }

        public IEnumerable<string> Codes()
        {
            var path = Configuration.Raw.daily;
            return Directory
                .GetFiles(path, "*.txt")
                .Select(Code)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();
        }

        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var file = Path.Combine(period.Path(LevelEnum.Level1), code + ".csv");

            return QueryFile(file);
        }

        public IEnumerable<StkDataSeries> Query(IEnumerable<string> codes, PeriodEnum period = PeriodEnum.Daily)
        {
            return codes.Distinct().Select(p => Query(p, period)).ToArray();
        }

        private string Code(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
        }

        private StkDataSeries QueryFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var name = Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
            var lines = File.ReadAllLines(path);

            var data = lines
                .Select(p =>
                {
                    var splits = p.Split(new[] { '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var isDate = Regex.IsMatch(splits[0], @"\d\d\d\d/\d\d/\d\d") || Regex.IsMatch(splits[0], @"\d\d/\d\d/\d\d\d\d");
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
}
