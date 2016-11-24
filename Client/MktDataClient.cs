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
using Interace.Mixin;

namespace Trade
{
    public class MktDataClient
    {
        public Fundamental QueryFundamental(string code)
        {
            return QueryFundamentals()
                .FirstOrDefault(p=>string.Equals(p.代码, code, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Fundamental> QueryFundamentals(IEnumerable<string> codes)
        {
            var set = QueryFundamentals();

            var q = from f in set
                    join c in codes on f.代码 equals c
                    select f;

            return q.ToArray();
        }

        public IEnumerable<Fundamental> QueryFundamentals()
        {
            return Configuration.level1.fundamental.ReadCsv<Fundamental>(StringSplitOptions.None);
        }

        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var path = Path.Combine(period.Path(LevelEnum.Level1), code + ".csv");

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

        public IEnumerable<StkDataSeries> Query(IEnumerable<string> codes, PeriodEnum period = PeriodEnum.Daily)
        {
            return codes.Distinct().Select(p => Query(p, period)).ToArray();
        }

        private string Code(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
        }
    }
}
