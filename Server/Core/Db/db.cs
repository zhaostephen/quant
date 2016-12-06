using Interace.Attribution;
using Interace.Mixin;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Data;

namespace Trade.Db
{
    public class db
    {
        public kdata kdata(string code, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "/" + code + ".csv");
            var p = file.ReadCsv<kdatapoint>();
            return new kdata(code, p);
        }

        public IEnumerable<Basics> basics()
        {
            var file = Configuration.data.basics.file("stock_basics.csv");
            return file.ReadCsv<Basics>();
        }

        public IEnumerable<string> codes()
        {
            return basics().Select(p => p.code).Distinct().ToArray();
        }
    }
}
