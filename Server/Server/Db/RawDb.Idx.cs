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
    partial class RawDb
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
    }
}
