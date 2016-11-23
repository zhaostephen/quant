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
    }
}
