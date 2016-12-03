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
using Excel;
using System.Data;
using Interace.Mixin;
using System.Text;
using Interace.Idx;

namespace Trade.Db
{
    public partial class RawDb
    {
        public void SaveIdx(Interace.Idx.Index rawIndx)
        {
            var path = Path.Combine(Configuration.Raw.index.EnsurePathCreated(), "last-update.csv");

            File.WriteAllText(path, rawIndx.LastUpdate.ToCsv(), Encoding.UTF8);
        }

        public Interace.Idx.Index GetIdx()
        {
            var path = Path.Combine(Configuration.Raw.index.EnsurePathCreated(), "last-update.csv");
            return new Interace.Idx.Index()
            {
                LastUpdate = path.ReadCsv<LastUpdateIdx>().ToArray()
            };
        }

        public DateTime? LastUpdate(string code, PeriodEnum period)
        {
            var path = PeriodPath(code, period);
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
                        var isDate = splits[0].Contains("/");
                        if (isDate)
                            dt = splits[0].Date();
                    }
                }
            }

            return dt;
        }
    }
}
