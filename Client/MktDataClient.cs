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
        public Fundamental QueryFundamental(string code)
        {
            return QueryFundamentals()
                .FirstOrDefault(p=>string.Equals(p.代码, code, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Fundamental> QueryFundamentals(IEnumerable<string> codes)
        {
            var set = QueryFundamentals();

            var q = from f in set
                    join c in codes on f.代码 equals c
                    select f;

            return q.ToArray();
        }

        public IEnumerable<Fundamental> QueryFundamentals()
        {
            return Configuration.level1.fundamental.ReadCsv<Fundamental>();
        }

        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var path = Path.Combine(period.Path(LevelEnum.Level1), code + ".csv");
            var p = path.ReadCsv<DataPoint>();

            return new StkDataSeries(code, new DataSeries(p), false);
        }

        public IEnumerable<StkDataSeries> Query(IEnumerable<string> codes, PeriodEnum period = PeriodEnum.Daily)
        {
            return codes.Distinct().Select(p => Query(p, period)).ToArray();
        }

        private string Code(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
        }
    }
}
