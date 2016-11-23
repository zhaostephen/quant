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
using Interace.Mixin;
using Interace.Idx;

namespace Trade.Db
{
    public partial class MktDb
    {
        public void SaveIdx(Interace.Idx.Index rawIndx)
        {
            var path = Path.Combine(Configuration.level1.index.EnsurePathCreated(), "last-update.csv");

            File.WriteAllText(path, rawIndx.LastUpdate.ToCsv(), Encoding.UTF8);
        }

        public Interace.Idx.Index GetIdx()
        {
            var path = Path.Combine(Configuration.level1.index.EnsurePathCreated(), "last-update.csv");
            return new Interace.Idx.Index()
            {
                LastUpdate = path.ReadCsv<LastUpdateIdx>().ToArray()
            };
        }

        public DateTime? LastUpdate(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var file = Path.Combine(period.Path(level), code + ".csv");
            return LastUpdate(file);
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
