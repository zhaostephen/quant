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

        public void UpdateIdx(Interace.Idx.Index index)
        {
            var i = GetIdx();

            var path = Path.Combine(Configuration.level1.index.EnsurePathCreated(), "last-update.csv");

            File.WriteAllText(path, i.LastUpdate.ToCsv(), Encoding.UTF8);
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

            DateTime? dt = null;

            using (var reader = new StreamReader(path))
            {
                var offset = Math.Min(reader.BaseStream.Length, 512);
                reader.BaseStream.Seek(offset * -1, SeekOrigin.End);

                while (!reader.EndOfStream)
                {
                    var splits = reader.ReadLine().Split(new[] { '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splits.Any())
                    {
                        var isDate = Regex.IsMatch(splits[0], @"\d\d\d\d/\d\d/\d\d") || Regex.IsMatch(splits[0], @"\d\d/\d\d/\d\d\d\d");
                        if (isDate)
                            dt = splits[0].Date();
                    }
                }
            }

            return dt;
        }
    }
}
