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
        internal void Save(IEnumerable<Fundamental> data)
        {
            var path = Configuration.level1.fundamental;
            while (true)
            {
                var dir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
                    break;
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(Path.Combine(path, "fundamental.csv"), data.ToCsv(), Encoding.UTF8);
        }
    }
}
