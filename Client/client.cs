using Trade.Cfg;
using Trade.Data;
using Trade.Mixin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interace.Mixin;

namespace Trade
{
    public class client
    {
        public string[] sectors()
        {
            return basics()
                .Select(p => p.sectors)
                .Where(p => !string.IsNullOrEmpty(p))
                .SelectMany(p => p.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToArray();
        }

        public Basic basics(string code)
        {
            return basics()
                .FirstOrDefault(p=>string.Equals(p.code, code, StringComparison.InvariantCultureIgnoreCase));
        }

        public Basics basics(IEnumerable<string> codes)
        {
            var set = basics();

            var q = from f in set
                    join c in codes on f.code equals c
                    select f;

            return new Basics(q.ToArray());
        }

        public Basics basics()
        {
            var file = Configuration.data.basics.file("basics.csv");
            return new Basics(file.ReadCsv<Basic>(Configuration.encoding.gbk));
        }

        public kdata kdata(string code, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "/" + code + ".csv");
            var p = file.ReadCsv<kdatapoint>(Configuration.encoding.gbk);
            return new kdata(code, p);
        }

        public IEnumerable<kdata> kdataall(string ktype, string secorOrIndex = null)
        {
            var codes = this.codes(secorOrIndex).ToArray();
            var results = codes
                .AsParallel()
                .Select(code => kdata(code, ktype))
                .Where(p => p != null)
                .ToArray();
            return results;
        }

        public IEnumerable<string> codes(string secorOrIndex = null)
        {
            return basics()
                .Where(p => string.IsNullOrEmpty(secorOrIndex) || p.belongtoindex(secorOrIndex) || p.belongtosector(secorOrIndex))
                .Select(p => p.code)
                .Distinct()
                .ToArray();
        }
    }
}
