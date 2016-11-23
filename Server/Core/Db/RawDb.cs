using System;
using System.Collections.Generic;
using System.Linq;
using Trade.Data;
using System.IO;
using System.Text.RegularExpressions;
using Trade.Mixin;
using Trade.Utility;
using log4net;
using ServiceStack;
using Trade.Cfg;
using Interace.Idx;

namespace Trade.Db
{
    public partial class RawDb
    {
        static ILog log = typeof(RawDb).Log();

        public StkDataSeries Query(string code, PeriodEnum period)
        {
            var path = PeriodPath(code, period);

            if (!File.Exists(path)) return null;
            var lines = File.ReadAllLines(path);

            var data = lines
                .Select(line => ParseData(line, period))
                .Where(p => p != null && p.Open > 0d && p.Close > 0d)
                .OrderBy(p => p.Date)
                .ToArray();

            if (!data.Any())
                return null;

            return new StkDataSeries(code, new DataSeries(data.NetPctChange()));
        }

        private DataPoint ParseData(string line, PeriodEnum period)
        {
            var splits = line.Split(new[] { '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!splits.Any()) return null;
            var isDate = Regex.IsMatch(splits[0], @"\d\d\d\d/\d\d/\d\d") || Regex.IsMatch(splits[0], @"\d\d/\d\d/\d\d\d\d");
            if (isDate)
            {
                if (period.Daybase())
                {
                    return new DataPoint
                    {
                        Date = splits[0].Date(),
                        Open = splits[1].Double(),
                        High = splits[2].Double(),
                        Low = splits[3].Double(),
                        Close = splits[4].Double()
                    };
                }
                else if(period.Minbase())
                {
                    var datetime = string.Format("{0} {1}:{2}", splits[0],splits[1].Substring(0,2), splits[1].Substring(2,2));
                    return new DataPoint
                    {
                        Date = datetime.Date(),
                        Open = splits[2].Double(),
                        High = splits[3].Double(),
                        Low = splits[4].Double(),
                        Close = splits[5].Double()
                    };
                }
            }

            return null;
        }
        private string PeriodPath(string code, PeriodEnum period)
        {
            var prefix = code.StartsWith("60") || code.StartsWith("88") ? "SH#" : "SZ#";
            return Path.Combine(period.Path(LevelEnum.Raw), prefix + code + ".txt");
        }
    }
}
