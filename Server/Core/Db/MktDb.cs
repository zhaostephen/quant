using Trade.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Trade.Mixin;
using Trade.Utility;
using log4net;
using ServiceStack;
using Trade.Cfg;

namespace Trade.Db
{
    public partial class MktDb
    {
        static ILog log = typeof(MktDb).Log();

        LevelEnum level;

        public MktDb(LevelEnum level = LevelEnum.Analytic)
        {
            this.level = level;
        }

        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var file = Path.Combine(period.Path(level), code + ".csv");

            return Query(file);
        }

        public void Save(IEnumerable<StkDataSeries> dataset, PeriodEnum period = PeriodEnum.Daily)
        {
            var path = period.Path(level);
            foreach (var d in dataset)
            {
                Save(d, Path.Combine(path, d.Code + ".csv"));
            }
        }

        private void Save(IEnumerable<object> data, string path)
        {
            while (true)
            {
                var dir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
                    break;
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, data.ToCsv(), Encoding.UTF8);
        }

        private StkDataSeries Query(string path)
        {
            if (!File.Exists(path)) return null;

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
