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
    public class MktDataClient
    {
        public Basics QueryFundamental(string code)
        {
            return QueryFundamentals()
                .FirstOrDefault(p=>string.Equals(p.code, code, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Basics> QueryFundamentals(IEnumerable<string> codes)
        {
            var set = QueryFundamentals();

            var q = from f in set
                    join c in codes on f.code equals c
                    select f;

            return q.ToArray();
        }

        public IEnumerable<Basics> QueryFundamentals()
        {
            return Configuration.level1.basics.ReadCsv<Basics>();
        }

        public kdata Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var path = Path.Combine(period.Path(LevelEnum.Analytic), code + ".csv");
            var p = path.ReadCsv<kdatapoint>();

            return new kdata(code, p);
        }

        public IEnumerable<kdata> Query(IEnumerable<string> codes, PeriodEnum period = PeriodEnum.Daily)
        {
            return codes.Distinct().Select(p => Query(p, period)).ToArray();
        }
    }
}
