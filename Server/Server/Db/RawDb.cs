using System;
using System.Collections.Generic;
using System.Linq;
using Screen.Data;
using System.IO;
using System.Text.RegularExpressions;
using Screen.Mixin;
using Screen.Utility;
using log4net;
using ServiceStack;
using Screen.Cfg;

namespace Screen.Db
{
    class RawDb
    {
        static ILog log = typeof(RawDb).Log();

        public string Code(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
        }

        public IEnumerable<string> Codes()
        {
            var path = Configuration.Raw.PATH;
            return Directory
                .GetFiles(path, "*.txt")
                .Select(Code)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();
        }

        public StkDataSeries QueryFile(string path)
        {
            return Query(path);
        }

        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            return Query(PeriodPath(code, period));
        }

        public DateTime? LastUpdate(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            return LastUpdate(PeriodPath(code, period));
        }

        private string PeriodPath(string code, PeriodEnum period)
        {
            var prefix = code.StartsWith("60") ? "SH#" : "SZ#";
            return Path.Combine(Configuration.Raw.PATH, prefix + code + ".txt");
        }

        private StkDataSeries Query(string path)
        {
            if (!File.Exists(path)) return null;

            var code = Code(path);
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

            return new StkDataSeries(code, new DataSeries(data.NetPctChange()));
        }

        private DateTime? LastUpdate(string path)
        {
            if (!File.Exists(path)) return null;

            var name = Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
            var lines = File.ReadAllLines(path);

            for (var i = lines.Length - 1; i >= 0; --i)
            {
                var splits = lines[i].Split(new[] { '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var isDate = Regex.IsMatch(splits[0], @"\d\d\d\d/\d\d/\d\d") || Regex.IsMatch(splits[0], @"\d\d/\d\d/\d\d\d\d");
                if (isDate)
                {
                    var p = new DataPoint
                    {
                        Date = splits[0].Date(),
                        Open = splits[1].Double(),
                        High = splits[2].Double(),
                        Low = splits[3].Double(),
                        Close = splits[4].Double()
                    };
                    if (p.Open > 0d && p.Close > 0d)
                        return p.Date;
                }
            }

            return null;
        }
    }
}
